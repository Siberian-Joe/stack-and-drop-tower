using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;
using Zenject;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class PlaceIntoTowerActionHandler : IDropActionHandler
    {
        public int Priority => 200;

        private readonly IGameplayWindowContext _windowContext;
        private readonly IBottomBarDragSession _session;
        private readonly ITowerPlacementRulesEvaluator _placementRules;
        private readonly ITowerStackState _towerStack;
        private readonly IActionInfoOverlay _actionInfoOverlay;

        public PlaceIntoTowerActionHandler(
            IGameplayWindowContext windowContext,
            IBottomBarDragSession session,
            ITowerPlacementRulesEvaluator placementRules,
            ITowerStackState towerStack,
            IActionInfoOverlay actionInfoOverlay)
        {
            _windowContext = windowContext;
            _session = session;
            _placementRules = placementRules;
            _towerStack = towerStack;
            _actionInfoOverlay = actionInfoOverlay;
        }

        public bool TryExecute()
        {
            var dragRect = _session.DragRect;
            if (_windowContext.TowerRoot == false || dragRect == false)
                return false;

            if (_towerStack == null || _placementRules == null)
                return false;

            var cam = _windowContext.UiCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_windowContext.TowerRoot, _session.LastScreenPoint, cam,
                    out var pointerLocal) == false)
            {
                return false;
            }

            var dropPivotPos = pointerLocal - _session.GrabLocalInSource;

            dragRect.SetParent(_windowContext.TowerRoot, false);
            dragRect.anchorMin = dragRect.anchorMax = new Vector2(0.5f, 0.5f);
            dragRect.localRotation = Quaternion.identity;
            dragRect.localScale = Vector3.one;
            dragRect.anchoredPosition = dropPivotPos;

            var width = dragRect.rect.width;
            var height = dragRect.rect.height;

            var context = new TowerPlacementRuleContext(
                screenPoint: _session.LastScreenPoint,
                desiredPivotPos: dropPivotPos,
                towerRoot: _windowContext.TowerRoot,
                uiCamera: cam,
                draggedColorId: _session.DraggedColorId,
                cubeWidth: width,
                cubeHeight: height,
                cubePivot: dragRect.pivot,
                isManualPlacement: true,
                requirePointerBeAboveTop: true,
                maxXOffsetFactor: _windowContext.MaxXOffsetFactor,
                stack: _towerStack);

            var ruleResult = _placementRules.Evaluate(context);
            if (ruleResult.IsSuccess == false)
            {
                _session.LastPlacementFailureKey = ruleResult.FailureLocalizationKey;
                return false;
            }

            if (TowerPlacementGeometry.TryComputeTarget(context, out var target) == false)
            {
                _session.LastPlacementFailureKey = "bottom_bar.rule.height_limit_reached";
                return false;
            }

            var start = target;
            var minFall = height * 0.75f;
            start.y = Mathf.Max(dropPivotPos.y, target.y + minFall);
            start.x = target.x;

            dragRect.DOKill();
            dragRect.anchoredPosition = start;
            dragRect
                .DOAnchorPos(target, _windowContext.FallDuration)
                .SetEase(_windowContext.FallEase);

            _towerStack.Add(new TowerCubeState(
                _session.DraggedColorId,
                dragRect,
                target,
                width,
                height));

            _actionInfoOverlay.Show("bottom_bar.action.placed_into_tower");
            return true;
        }
    }
}