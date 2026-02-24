using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using UnityEngine;
using Zenject;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class PlaceIntoTowerActionHandler : IDropActionHandler
    {
        public int Priority => 200;

        private readonly BottomBarDragDropRefs _refs;
        private readonly BottomBarDragSession _session;
        private readonly ITowerPlacementRulesEvaluator _placementRules;
        private readonly ITowerStackState _towerStack;

        [Inject]
        public PlaceIntoTowerActionHandler(
            BottomBarDragDropRefs refs,
            BottomBarDragSession session,
            ITowerPlacementRulesEvaluator placementRules,
            ITowerStackState towerStack)
        {
            _refs = refs;
            _session = session;
            _placementRules = placementRules;
            _towerStack = towerStack;
        }

        public bool TryExecute()
        {
            var dragRect = _session.DragRect;
            if (_refs.TowerDropArea == null || _refs.TowerRoot == null || dragRect == null)
                return false;

            if (_towerStack == null || _placementRules == null)
                return false;

            var cam = _refs.UiCamera;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _refs.TowerRoot, _session.LastScreenPoint, cam, out var pointerLocal))
            {
                return false;
            }

            var dropPivotPos = pointerLocal - _session.GrabLocalInSource;

            dragRect.SetParent(_refs.TowerRoot, worldPositionStays: false);
            dragRect.anchorMin = dragRect.anchorMax = new Vector2(0.5f, 0.5f);
            dragRect.localRotation = Quaternion.identity;
            dragRect.localScale = Vector3.one;
            dragRect.anchoredPosition = dropPivotPos;

            var w = dragRect.rect.width;
            var h = dragRect.rect.height;

            var context = new TowerPlacementRuleContext(
                screenPoint: _session.LastScreenPoint,
                desiredPivotPos: dropPivotPos,
                towerDropArea: _refs.TowerDropArea,
                towerRoot: _refs.TowerRoot,
                uiCamera: cam,
                draggedColorId: _session.DraggedColorId,
                cubeWidth: w,
                cubeHeight: h,
                cubePivot: dragRect.pivot,
                isManualPlacement: true,
                requirePointerBeAboveTop: true,
                maxXOffsetFactor: _refs.MaxXOffsetFactor,
                stack: _towerStack);

            var ruleResult = _placementRules.Evaluate(context);
            if (!ruleResult.IsSuccess)
            {
                _session.LastPlacementFailureKey = ruleResult.FailureLocalizationKey;
                return false;
            }

            if (!TowerPlacementGeometry.TryComputeTarget(context, out var target))
            {
                _session.LastPlacementFailureKey = "bottom_bar.rule.cube_does_not_fit";
                return false;
            }

            var start = target;
            var minFall = h * 0.75f;
            start.y = Mathf.Max(dropPivotPos.y, target.y + minFall);
            start.x = target.x;

            dragRect.DOKill();
            dragRect.anchoredPosition = start;
            dragRect
                .DOAnchorPos(target, _refs.FallDuration)
                .SetEase(_refs.FallEase);

            _towerStack.Add(new TowerCubeState(
                colorId: _session.DraggedColorId,
                rect: dragRect,
                target: target,
                width: w,
                height: h));

            return true;
        }
    }
}