using Game.UI.BottomBar.Runtime;
using Game.UI.Screens.Contracts;
using UnityEngine;

namespace Game.UI.Screens.Runtime.Views
{
    public sealed class GameplayWindowView : WindowScreenView
    {
        [Header("Bottom bar")] [SerializeField]
        private BottomBarView _bottomBar;

        [Header("Drag")] [SerializeField] private RectTransform _dragLayer;

        [Header("Tower")] [SerializeField] private RectTransform _towerRoot;

        [Header("Hole")] [SerializeField] private RectTransform _holeArea;
        [SerializeField] private RectTransform _holeMaskRoot;
        [SerializeField] private RectTransform _holeMouth;

        public BottomBarView BottomBar => _bottomBar;

        public RectTransform DragLayer => _dragLayer;

        public RectTransform TowerRoot => _towerRoot;

        public RectTransform HoleArea => _holeArea;
        public RectTransform HoleMaskRoot => _holeMaskRoot;
        public RectTransform HoleMouth => _holeMouth;
    }
}