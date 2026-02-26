using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class BottomBarDragInteractor : ICubeDragInteractor
    {
        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly IBottomBarDragSession _session;
        private readonly ITowerPlacementRulesEvaluator _placementRules;
        private readonly ITowerStackState _towerStack;
        private readonly ICubeViewFactory _cubeFactory;
        private readonly IReadOnlyList<IDropActionHandler> _dropActionHandlers;

        public BottomBarDragInteractor(
            IGameplayWindowContext gameplayWindow,
            IBottomBarDragSession session,
            ITowerPlacementRulesEvaluator placementRules,
            ITowerStackState towerStack,
            ICubeViewFactory cubeFactory,
            List<IDropActionHandler> dropActionHandlers)
        {
            _gameplayWindow = gameplayWindow;
            _session = session;
            _placementRules = placementRules;
            _towerStack = towerStack;
            _cubeFactory = cubeFactory;
            _dropActionHandlers = dropActionHandlers
                .OrderBy(handler => handler.Priority)
                .ToArray();
        }

        public void BeginDrag(CubeView source, int pointerId, Vector2 screenPoint)
        {
            if (_session.IsDragging || source == false)
                return;

            _session.ActivePointerId = pointerId;
            _session.LastScreenPoint = screenPoint;
            _session.Origin = BottomBarDragOrigin.None;
            _session.DraggedColorId = source.ColorId ?? string.Empty;
            _session.LastPlacementFailureKey = null;

            LockScroll();

            var sourceRt = (RectTransform)source.transform;
            var cam = _gameplayWindow.UiCamera;

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
            if (_session.IsDragging == false)
                return;

            _session.LastScreenPoint = screenPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameplayWindow.DragLayer, screenPoint,
                    _gameplayWindow.UiCamera, out var pointerLocal))
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
                if (_dropActionHandlers.Any(handler => handler.TryExecute()) == false)
                    return;

                PromoteDraggedCubeToTowerIfNeeded();
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

            var cloneView = _cubeFactory.Clone(source, _gameplayWindow.DragLayer);
            if (cloneView == false)
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
                _gameplayWindow.DragLayer, screenPoint, _gameplayWindow.UiCamera, out var pointerLocal);

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

            if (extractedIndex == 0)
            {
                foreach (var cube in above)
                    PlayFailCollapseAndDestroy(cube);

                return;
            }

            foreach (var cube in above)
                RevalidateAndCollapseCube(cube);
        }

        private void RevalidateAndCollapseCube(TowerCubeState cube)
        {
            if (cube.Rect == false || _towerStack == null || _placementRules == null)
                return;

            cube.Rect.DOKill();

            var context = new TowerPlacementRuleContext(
                screenPoint: default,
                desiredPivotPos: cube.Target,
                towerRoot: _gameplayWindow.TowerRoot,
                uiCamera: _gameplayWindow.UiCamera,
                draggedColorId: cube.ColorId,
                cubeWidth: cube.Width,
                cubeHeight: cube.Height,
                cubePivot: cube.Rect.pivot,
                isManualPlacement: false,
                requirePointerBeAboveTop: false,
                maxXOffsetFactor: _gameplayWindow.MaxXOffsetFactor,
                stack: _towerStack);

            var ruleResult = _placementRules.Evaluate(context);
            if (ruleResult.IsSuccess == false ||
                TowerPlacementGeometry.TryComputeTarget(context, out var newTarget) == false)
            {
                PlayFailFallAndDestroy(cube.Rect.gameObject, cube.Rect);
                return;
            }

            var minFall = cube.Height * 0.75f;
            var current = cube.Rect.anchoredPosition;

            var start = new Vector2(
                newTarget.x,
                Mathf.Max(current.y, newTarget.y + minFall));

            var approachDuration = Mathf.Clamp(_gameplayWindow.FallDuration * 0.35f, 0.05f, 0.12f);

            BottomBarDragVisualUtility.PlayApproachThenFall(
                rect: cube.Rect,
                approachPos: start,
                targetPos: newTarget,
                approachDuration: approachDuration,
                approachEase: Ease.OutQuad,
                fallDuration: _gameplayWindow.FallDuration,
                fallEase: _gameplayWindow.FallEase);

            _towerStack.Add(cube.WithTarget(newTarget));
        }

        private void ReparentDraggedRectToDragLayer(Vector2 screenPoint)
        {
            var dragRect = _session.DragRect;
            if (dragRect == false || _gameplayWindow.DragLayer == false)
                return;

            dragRect.SetParent(_gameplayWindow.DragLayer, worldPositionStays: false);
            dragRect.SetAsLastSibling();

            dragRect.anchorMin = dragRect.anchorMax = new Vector2(0.5f, 0.5f);
            dragRect.localRotation = Quaternion.identity;
            dragRect.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _gameplayWindow.DragLayer, screenPoint, _gameplayWindow.UiCamera, out var pointerLocal);

            dragRect.anchoredPosition = pointerLocal - _session.GrabLocalInSource;
        }

        private void PromoteDraggedCubeToTowerIfNeeded()
        {
            var dragObject = _session.DragObject;
            var dragRect = _session.DragRect;

            if (dragObject == false || dragRect == false || _gameplayWindow.TowerRoot == false)
                return;

            if (!dragRect.IsChildOf(_gameplayWindow.TowerRoot))
                return;

            BottomBarDragVisualUtility.SetGraphicRaycasts(dragObject, true);

            var cubeView = dragObject.GetComponent<CubeView>();
            if (cubeView == false)
                return;

            cubeView.enabled = true;
            cubeView.Setup(this);
        }

        private void PlayFailFallAndDestroy(GameObject obj, RectTransform rect)
        {
            if (obj == false || rect == false || _gameplayWindow.DragLayer == false)
                return;

            var cam = _gameplayWindow.UiCamera;
            var screen = RectTransformUtility.WorldToScreenPoint(cam, rect.position);

            rect.SetParent(_gameplayWindow.DragLayer, worldPositionStays: false);
            rect.SetAsLastSibling();

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameplayWindow.DragLayer, screen, cam,
                out var local);
            rect.anchoredPosition = local;

            var endY = _gameplayWindow.DragLayer.rect.yMin - rect.rect.height - _gameplayWindow.FailFallExtra;

            rect.DOKill();
            rect.DOAnchorPosY(endY, _gameplayWindow.FailFallDuration)
                .SetEase(_gameplayWindow.FailFallEase)
                .OnComplete(() =>
                {
                    if (obj)
                        Object.Destroy(obj);
                });
        }

        private bool IsTowerCube(RectTransform rect) =>
            rect && _gameplayWindow.TowerRoot && rect.IsChildOf(_gameplayWindow.TowerRoot);

        private void ResetAfterFailedBegin()
        {
            _session.Reset();
            UnlockScroll();
        }

        private void LockScroll()
        {
            if (_gameplayWindow.ScrollRect == false)
                return;

            _gameplayWindow.ScrollRect.StopMovement();
            _gameplayWindow.ScrollRect.enabled = false;
        }

        private void UnlockScroll()
        {
            if (_gameplayWindow.ScrollRect == false)
                return;

            _gameplayWindow.ScrollRect.enabled = true;
        }

        private void PlayFailCollapseAndDestroy(in TowerCubeState cube)
        {
            if (cube.Rect == false)
                return;

            var obj = cube.Rect.gameObject;
            if (obj == false)
                return;

            BottomBarDragVisualUtility.SetGraphicRaycasts(obj, false);

            PlayFailFallAndDestroy(obj, cube.Rect);
        }
    }
}