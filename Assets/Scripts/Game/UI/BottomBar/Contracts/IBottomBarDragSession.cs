using UnityEngine;

namespace Game.UI.BottomBar.Contracts
{
    public interface IBottomBarDragSession
    {
        GameObject DragObject { get; set; }
        RectTransform DragRect { get; set; }

        Vector2 GrabLocalInSource { get; set; }
        Vector2 LastScreenPoint { get; set; }
        int ActivePointerId { get; set; }
        BottomBarDragOrigin Origin { get; set; }

        string DraggedColorId { get; set; }
        string LastPlacementFailureKey { get; set; }

        bool IsDragging { get; }

        void Reset();
    }
}