using UnityEngine;

namespace Game.UI.BottomBar.Contracts
{
    public readonly struct TowerPlacementRuleContext
    {
        public Vector2 ScreenPoint { get; }
        public Vector2 DesiredPivotPos { get; }

        public RectTransform TowerRoot { get; }
        public Camera UiCamera { get; }

        public string DraggedColorId { get; }
        public float CubeWidth { get; }
        public float CubeHeight { get; }
        public Vector2 CubePivot { get; }

        public bool IsManualPlacement { get; }
        public bool RequirePointerBeAboveTop { get; }

        public float MaxXOffsetFactor { get; }

        public ITowerStackState Stack { get; }

        public TowerPlacementRuleContext(
            Vector2 screenPoint,
            Vector2 desiredPivotPos,
            RectTransform towerRoot,
            Camera uiCamera,
            string draggedColorId,
            float cubeWidth,
            float cubeHeight,
            Vector2 cubePivot,
            bool isManualPlacement,
            bool requirePointerBeAboveTop,
            float maxXOffsetFactor,
            ITowerStackState stack)
        {
            ScreenPoint = screenPoint;
            DesiredPivotPos = desiredPivotPos;
            TowerRoot = towerRoot;
            UiCamera = uiCamera;
            DraggedColorId = draggedColorId ?? string.Empty;
            CubeWidth = cubeWidth;
            CubeHeight = cubeHeight;
            CubePivot = cubePivot;
            IsManualPlacement = isManualPlacement;
            RequirePointerBeAboveTop = requirePointerBeAboveTop;
            MaxXOffsetFactor = maxXOffsetFactor;
            Stack = stack;
        }
    }
}