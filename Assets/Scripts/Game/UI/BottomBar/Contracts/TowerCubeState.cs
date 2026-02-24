using UnityEngine;

namespace Game.UI.BottomBar.Contracts
{
    public readonly struct TowerCubeState
    {
        public string ColorId { get; }
        public RectTransform Rect { get; }
        public Vector2 Target { get; }
        public float Width { get; }
        public float Height { get; }

        public TowerCubeState(
            string colorId,
            RectTransform rect,
            Vector2 target,
            float width,
            float height)
        {
            ColorId = colorId ?? string.Empty;
            Rect = rect;
            Target = target;
            Width = width;
            Height = height;
        }

        public TowerCubeState WithTarget(Vector2 target) =>
            new(ColorId, Rect, target, Width, Height);
    }
}