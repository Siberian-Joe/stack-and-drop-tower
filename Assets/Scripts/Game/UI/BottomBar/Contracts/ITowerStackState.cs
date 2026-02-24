using UnityEngine;

namespace Game.UI.BottomBar.Contracts
{
    public interface ITowerStackState
    {
        int Count { get; }
        bool HasTop { get; }
        TowerCubeState Top { get; }

        int IndexOf(RectTransform rect);
        void Add(in TowerCubeState cube);

        TowerCubeState[] ExtractAtAndRemoveAbove(int index, out TowerCubeState extractedCube);
    }
}