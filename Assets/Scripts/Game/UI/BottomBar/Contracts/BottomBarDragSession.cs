using UnityEngine;

namespace Game.UI.BottomBar.Contracts
{
    public sealed class BottomBarDragSession
    {
        public GameObject DragObject { get; set; }
        public RectTransform DragRect { get; set; }

        public Vector2 GrabLocalInSource { get; set; }
        public Vector2 LastScreenPoint { get; set; }
        public int ActivePointerId { get; set; } = int.MinValue;
        public BottomBarDragOrigin Origin { get; set; } = BottomBarDragOrigin.None;

        public string DraggedColorId { get; set; } = string.Empty;
        public string LastPlacementFailureKey { get; set; }

        public bool IsDragging => DragObject != null && DragRect != null;

        public void Reset()
        {
            DragObject = null;
            DragRect = null;
            GrabLocalInSource = default;
            LastScreenPoint = default;
            ActivePointerId = int.MinValue;
            Origin = BottomBarDragOrigin.None;
            DraggedColorId = string.Empty;
            LastPlacementFailureKey = null;
        }
    }
}