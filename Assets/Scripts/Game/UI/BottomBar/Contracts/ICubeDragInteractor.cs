using Game.UI.BottomBar.Runtime;
using UnityEngine;

namespace Game.UI.BottomBar.Contracts
{
    public interface ICubeDragInteractor
    {
        void BeginDrag(CubeView source, int pointerId, Vector2 screenPoint);
        void Move(Vector2 screenPoint);
        void EndDrag(int pointerId);
    }
}