using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class BottomBarDragDropRefs : MonoBehaviour
    {
        [Header("BottomBar")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _dragLayer;
        [SerializeField] private Canvas _canvas;

        [Header("Tower")]
        [SerializeField] private RectTransform _towerRoot;
        [SerializeField, Range(0.1f, 1f)] private float _maxXOffsetFactor = 0.5f;
        [SerializeField, Min(0.01f)] private float _fallDuration = 0.25f;
        [SerializeField] private Ease _fallEase = Ease.OutQuad;

        [Header("Hole")]
        [SerializeField] private RectTransform _holeArea;
        [SerializeField] private RectTransform _holeMaskRoot;
        [SerializeField] private RectTransform _holeMouth;
        [SerializeField, Range(0.5f, 1f)] private float _holeEllipsePadding = 0.9f;
        [SerializeField, Min(0.01f)] private float _holePullDuration = 0.12f;
        [SerializeField] private Ease _holePullEase = Ease.OutQuad;
        [SerializeField, Min(0.01f)] private float _holeFallDuration = 0.25f;
        [SerializeField] private Ease _holeFallEase = Ease.InQuad;
        [SerializeField, Min(0f)] private float _holeFallExtra = 60f;

        [Header("Fail feedback")]
        [SerializeField, Min(0.01f)] private float _failFallDuration = 0.35f;
        [SerializeField] private Ease _failFallEase = Ease.InQuad;
        [SerializeField, Min(0f)] private float _failFallExtra = 80f;

        public ScrollRect ScrollRect => _scrollRect;
        public RectTransform DragLayer => _dragLayer;
        public Canvas Canvas => _canvas;

        public RectTransform TowerRoot => _towerRoot;
        public float MaxXOffsetFactor => _maxXOffsetFactor;
        public float FallDuration => _fallDuration;
        public Ease FallEase => _fallEase;

        public RectTransform HoleArea => _holeArea;
        public RectTransform HoleMaskRoot => _holeMaskRoot;
        public RectTransform HoleMouth => _holeMouth;
        public float HoleEllipsePadding => _holeEllipsePadding;
        public float HolePullDuration => _holePullDuration;
        public Ease HolePullEase => _holePullEase;
        public float HoleFallDuration => _holeFallDuration;
        public Ease HoleFallEase => _holeFallEase;
        public float HoleFallExtra => _holeFallExtra;

        public float FailFallDuration => _failFallDuration;
        public Ease FailFallEase => _failFallEase;
        public float FailFallExtra => _failFallExtra;

        public Camera UiCamera =>
            _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;
    }
}