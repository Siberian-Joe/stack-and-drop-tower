using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class PlaceIntoTowerActionHandler : IDropActionHandler
    {
        public int Priority => 200;

        private const int RandomTargetAttempts = 16;

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

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _windowContext.TowerRoot,
                    _session.LastScreenPoint,
                    cam,
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

            var baseContext = new TowerPlacementRuleContext(
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

            var initialRules = _placementRules.Evaluate(baseContext);
            if (initialRules.IsSuccess == false)
            {
                _session.LastPlacementFailureKey = initialRules.FailureLocalizationKey;
                return false;
            }

            var randomizeXOnStack = _towerStack.Count > 0;

            if (TryPickValidTarget(baseContext, randomizeXOnStack, out var target, out var failureKey) == false)
            {
                _session.LastPlacementFailureKey = failureKey ?? "bottom_bar.rule.height_limit_reached";
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

        private bool TryPickValidTarget(
            in TowerPlacementRuleContext baseContext,
            bool randomizeXOnStack,
            out Vector2 target,
            out string failureKey)
        {
            failureKey = null;

            if (!randomizeXOnStack)
            {
                if (TowerPlacementGeometry.TryComputeTarget(baseContext, out target) == false)
                {
                    failureKey = "bottom_bar.rule.height_limit_reached";
                    return false;
                }

                var res = _placementRules.Evaluate(WithDesiredPivotPos(baseContext, target));
                if (res.IsSuccess)
                    return true;

                failureKey = res.FailureLocalizationKey;
                return false;
            }

            var top = baseContext.Stack.Top;

            var xMin = top.Target.x - top.Width * baseContext.MaxXOffsetFactor;
            var xMax = top.Target.x + top.Width * baseContext.MaxXOffsetFactor;

            string lastFailure = null;

            for (var i = 0; i < RandomTargetAttempts; i++)
            {
                var candidateX = Random.Range(xMin, xMax);

                var candidateContext = WithDesiredPivotPos(baseContext,
                    new Vector2(candidateX, baseContext.DesiredPivotPos.y));

                if (TowerPlacementGeometry.TryComputeTarget(candidateContext, out var candidateTarget) == false)
                {
                    lastFailure = "bottom_bar.rule.height_limit_reached";
                    continue;
                }

                var res = _placementRules.Evaluate(WithDesiredPivotPos(baseContext, candidateTarget));
                if (res.IsSuccess)
                {
                    target = candidateTarget;
                    return true;
                }

                lastFailure = res.FailureLocalizationKey;
            }

            if (TowerPlacementGeometry.TryComputeTarget(baseContext, out target) == false)
            {
                failureKey = "bottom_bar.rule.height_limit_reached";
                return false;
            }

            var fallbackRes = _placementRules.Evaluate(WithDesiredPivotPos(baseContext, target));
            if (fallbackRes.IsSuccess)
                return true;

            failureKey = fallbackRes.FailureLocalizationKey ?? lastFailure;
            return false;
        }

        private static TowerPlacementRuleContext WithDesiredPivotPos(
            in TowerPlacementRuleContext source,
            Vector2 desiredPivotPos)
        {
            return new TowerPlacementRuleContext(
                screenPoint: source.ScreenPoint,
                desiredPivotPos: desiredPivotPos,
                towerRoot: source.TowerRoot,
                uiCamera: source.UiCamera,
                draggedColorId: source.DraggedColorId,
                cubeWidth: source.CubeWidth,
                cubeHeight: source.CubeHeight,
                cubePivot: source.CubePivot,
                isManualPlacement: source.IsManualPlacement,
                requirePointerBeAboveTop: source.RequirePointerBeAboveTop,
                maxXOffsetFactor: source.MaxXOffsetFactor,
                stack: source.Stack);
        }
    }
}