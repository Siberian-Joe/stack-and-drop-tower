using Game.UI.Screens.Contracts;
using UnityEngine;

namespace Game.UI.Screens.Runtime
{
    public sealed class ScreenRoots : MonoBehaviour, IScreenRoots
    {
        [SerializeField] private RectTransform _windowRoot;
        [SerializeField] private RectTransform _overlayRoot;
        [SerializeField] private RectTransform _cacheRoot;
        [SerializeField] private Canvas _canvas;

        public RectTransform WindowRoot => _windowRoot;
        public RectTransform OverlayRoot => _overlayRoot;
        public RectTransform CacheRoot => _cacheRoot;
        public Canvas Canvas => _canvas;
    }
}