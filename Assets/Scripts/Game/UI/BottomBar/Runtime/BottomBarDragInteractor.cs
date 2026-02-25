using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using UnityEngine;
using Zenject;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class BottomBarDragInteractor : ICubeDragInteractor
    {
        private readonly BottomBarDragDropRefs _refs;
        private readonly BottomBarDragSession _session;
        private readonly ITowerPlacementRulesEvaluator _placementRules;
        private readonly ITowerStackState _towerStack;
        private readonly ICubeViewFactory _cubeFactory;
        private readonly IReadOnlyList<IDropActionHandler> _dropActionHandlers;

        public BottomBarDragInteractor(
            BottomBarDragDropRefs refs,
            BottomBarDragSession session,
            ITowerPlacementRulesEvaluator placementRules,
            ITowerStackState towerStack,
            ICubeViewFactory cubeFactory,
            List<IDropActionHandler> dropActionHandlers)
        {
            _refs = refs;
            _session = session;
            _placementRules = placementRules;
            _towerStack = towerStack;
            _cubeFactory = cubeFactory;
            _dropActionHandlers = dropActionHandlers
                .OrderBy(x => x.Priority)
                .ToArray();
        }

        public void BeginDrag(CubeView source, int pointerId, Vector2 screenPoint)
        {
            if (_session.IsDragging || source == null)
                return;

            _session.ActivePointerId = pointerId;
            _session.LastScreenPoint = screenPoint;
            _session.Origin = BottomBarDragOrigin.None;
            _session.DraggedColorId = source.ColorId ?? string.Empty;
            _session.LastPlacementFailureKey = null;

            LockScroll();

            var sourceRt = (RectTransform)source.transform;
            var cam = _refs.UiCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                sourceRt, screenPoint, cam, out var grabLocal);

            _session.GrabLocalInSource = grabLocal;

            if (IsTowerCube(sourceRt))
            {
                if (BeginDragFromTower(sourceRt, screenPoint))
                    return;

                ResetAfterFailedBegin();
                return;
            }

            BeginDragFromBottomBar(source, sourceRt, screenPoint);
        }

        public void Move(Vector2 screenPoint)
        {
            if (!_session.IsDragging)
                return;

            _session.LastScreenPoint = screenPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _refs.DragLayer, screenPoint, _refs.UiCamera, out var pointerLocal))
            {
                _session.DragRect.anchoredPosition = pointerLocal - _session.GrabLocalInSource;
            }
        }

        public void EndDrag(int pointerId)
        {
            if (!_session.IsDragging || pointerId != _session.ActivePointerId)
                return;

            try
            {
                foreach (var handler in _dropActionHandlers)
                {
                    if (handler.TryExecute())
                    {
                        PromoteDraggedCubeToTowerIfNeeded();
                        return;
                    }
                }
            }
            finally
            {
                _session.Reset();
                UnlockScroll();
            }
        }

        private void BeginDragFromBottomBar(CubeView source, RectTransform sourceRt, Vector2 screenPoint)
        {
            _session.Origin = BottomBarDragOrigin.BottomBarClone;

            var cloneView = _cubeFactory.Clone(source, _refs.DragLayer);
            if (cloneView == null)
            {
                ResetAfterFailedBegin();
                return;
            }

            var clone = cloneView.gameObject;
            var dragRect = (RectTransform)clone.transform;
            dragRect.SetAsLastSibling();

            cloneView.enabled = false;

            BottomBarDragVisualUtility.NormalizeRectForDrag(dragRect, sourceRt);
            BottomBarDragVisualUtility.SetGraphicRaycasts(clone, false);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _refs.DragLayer, screenPoint, _refs.UiCamera, out var pointerLocal);

            dragRect.anchoredPosition = pointerLocal - _session.GrabLocalInSource;

            _session.DragObject = clone;
            _session.DragRect = dragRect;
        }

        private bool BeginDragFromTower(RectTransform sourceRt, Vector2 screenPoint)
        {
            var index = _towerStack?.IndexOf(sourceRt) ?? -1;
            if (index < 0)
                return false;

            _session.Origin = BottomBarDragOrigin.TowerExtracted;
            _session.DragObject = sourceRt.gameObject;
            _session.DragRect = sourceRt;

            sourceRt.DOKill();
            BottomBarDragVisualUtility.SetGraphicRaycasts(sourceRt.gameObject, false);

            ExtractCubeAndCollapseAbove(index);
            ReparentDraggedRectToDragLayer(screenPoint);

            return true;
        }

        private void ExtractCubeAndCollapseAbove(int extractedIndex)
        {
            if (_towerStack == null)
                return;

            if (extractedIndex < 0 || extractedIndex >= _towerStack.Count)
                return;

            var above = _towerStack.ExtractAtAndRemoveAbove(extractedIndex, out _);

            foreach (var cube in above)
                RevalidateAndCollapseCube(cube);
        }

        private void RevalidateAndCollapseCube(TowerCubeState cube)
        {
            if (cube.Rect == null || _towerStack == null || _placementRules == null)
                return;

            cube.Rect.DOKill();

            var context = new TowerPlacementRuleContext(
                screenPoint: default,
                desiredPivotPos: cube.Target,
                towerDropArea: _refs.TowerDropArea,
                towerRoot: _refs.TowerRoot,
                uiCamera: _refs.UiCamera,
                draggedColorId: cube.ColorId,
                cubeWidth: cube.Width,
                cubeHeight: cube.Height,
                cubePivot: cube.Rect.pivot,
                isManualPlacement: false,
                requirePointerBeAboveTop: false,
                maxXOffsetFactor: _refs.MaxXOffsetFactor,
                stack: _towerStack);

            var ruleResult = _placementRules.Evaluate(context);
            if (!ruleResult.IsSuccess)
            {
                PlayFailFallAndDestroy(cube.Rect.gameObject, cube.Rect);
                return;
            }

            if (!TowerPlacementGeometry.TryComputeTarget(context, out var newTarget))
            {
                PlayFailFallAndDestroy(cube.Rect.gameObject, cube.Rect);
                return;
            }

            var start = cube.Rect.anchoredPosition;
            if (start.y < newTarget.y)
            {
                start.y = newTarget.y + cube.Height * 0.75f;
                cube.Rect.anchoredPosition = start;
            }

            cube.Rect
                .DOAnchorPos(newTarget, _refs.FallDuration)
                .SetEase(_refs.FallEase);

            _towerStack.Add(cube.WithTarget(newTarget));
        }

        private void ReparentDraggedRectToDragLayer(Vector2 screenPoint)
        {
            var dragRect = _session.DragRect;
            if (dragRect == null || _refs.DragLayer == null)
                return;

            dragRect.SetParent(_refs.DragLayer, worldPositionStays: false);
            dragRect.SetAsLastSibling();

            dragRect.anchorMin = dragRect.anchorMax = new Vector2(0.5f, 0.5f);
            dragRect.localRotation = Quaternion.identity;
            dragRect.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _refs.DragLayer, screenPoint, _refs.UiCamera, out var pointerLocal);

            dragRect.anchoredPosition = pointerLocal - _session.GrabLocalInSource;
        }

        private void PromoteDraggedCubeToTowerIfNeeded()
        {
            var dragObject = _session.DragObject;
            var dragRect = _session.DragRect;

            if (dragObject == null || dragRect == null || _refs.TowerRoot == null)
                return;

            if (!dragRect.IsChildOf(_refs.TowerRoot))
                return;

            BottomBarDragVisualUtility.SetGraphicRaycasts(dragObject, true);

            var cubeView = dragObject.GetComponent<CubeView>();
            if (cubeView != null)
            {
                cubeView.enabled = true;
                cubeView.Setup(this, null); // tower mode
            }
        }

        private void PlayFailFallAndDestroy(GameObject obj, RectTransform rect)
        {
            if (obj == null || rect == null || _refs.DragLayer == null)
                return;

            var cam = _refs.UiCamera;
            var screen = RectTransformUtility.WorldToScreenPoint(cam, rect.position);

            rect.SetParent(_refs.DragLayer, worldPositionStays: false);
            rect.SetAsLastSibling();

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_refs.DragLayer, screen, cam, out var local);
            rect.anchoredPosition = local;

            var endY = _refs.DragLayer.rect.yMin - rect.rect.height - _refs.FailFallExtra;

            rect.DOKill();
            rect.DOAnchorPosY(endY, _refs.FailFallDuration)
                .SetEase(_refs.FailFallEase)
                .OnComplete(() =>
                {
                    if (obj != null)
                        Object.Destroy(obj);
                });
        }

        private bool IsTowerCube(RectTransform rect)
            => rect != null && _refs.TowerRoot != null && rect.IsChildOf(_refs.TowerRoot);

        private void ResetAfterFailedBegin()
        {
            _session.Reset();
            UnlockScroll();
        }

        private void LockScroll()
        {
            if (_refs.ScrollRect == null) return;
            _refs.ScrollRect.StopMovement();
            _refs.ScrollRect.enabled = false;
        }

        private void UnlockScroll()
        {
            if (_refs.ScrollRect == null) return;
            _refs.ScrollRect.enabled = true;
        }
    }
}