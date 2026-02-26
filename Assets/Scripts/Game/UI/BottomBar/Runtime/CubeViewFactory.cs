using System;
using System.Linq;
using DG.Tweening;
using Game.Config.Contracts;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class CubeViewFactory : ICubeViewFactory, IDisposable
    {
        private const int DefaultCapacity = 32;
        private const int MaxPoolSize = 256;

        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly IGameConfigReader _config;

        private ObjectPool<CubeView> _pool;
        private Transform _poolRoot;
        private CubeView _prefab;
        private RectTransform _prefabRect;

        public CubeViewFactory(
            IGameplayWindowContext gameplayWindow,
            IGameConfigReader config)
        {
            _gameplayWindow = gameplayWindow;
            _config = config;
        }

        public CubeView Create(ICubeColorDefinition definition, Transform parent)
        {
            if (definition == null || parent == false)
                return null;

            if (TryEnsurePool() == false)
                return null;

            var cube = _pool.Get();
            PrepareForSpawn(cube, parent);

            cube.enabled = true;
            cube.Bind(definition);
            BottomBarDragVisualUtility.SetGraphicRaycasts(cube.gameObject, true);

            return cube;
        }

        public CubeView CreateByColorId(string colorId, Transform parent)
        {
            if (string.IsNullOrWhiteSpace(colorId))
                return null;

            var colors = _config.Current.BottomBar.Colors;
            return (from definition in colors
                    where definition != null && definition.Id == colorId
                    select Create(definition, parent))
                .FirstOrDefault();
        }

        public CubeView Clone(CubeView source, Transform parent)
        {
            if (source == false || parent == false)
                return null;

            return CreateByColorId(source.ColorId, parent);
        }

        public void Release(CubeView cube)
        {
            if (cube == false)
                return;

            if (_pool == null)
            {
                Object.Destroy(cube.gameObject);
                return;
            }

            _pool.Release(cube);
        }

        private bool TryEnsurePool()
        {
            if (_pool != null)
                return true;

            _prefab = _gameplayWindow.BottomBarView
                ? _gameplayWindow.BottomBarView.CubePrefab
                : null;

            if (_prefab == false)
                return false;

            _prefabRect = _prefab?.transform as RectTransform;
            _poolRoot = EnsurePoolRoot();

            _pool = new ObjectPool<CubeView>(
                createFunc: CreatePooled,
                actionOnGet: OnGet,
                actionOnRelease: OnReleaseInternal,
                actionOnDestroy: OnDestroyPooled,
                collectionCheck: true,
                defaultCapacity: DefaultCapacity,
                maxSize: MaxPoolSize);

            return true;
        }

        private Transform EnsurePoolRoot()
        {
            Transform parent = _gameplayWindow.TowerRoot
                ? _gameplayWindow.TowerRoot
                : null;

            if (parent == false)
                return null;

            var gameObject = new GameObject("CubePool");
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.SetAsFirstSibling();
            return gameObject.transform;
        }

        private CubeView CreatePooled()
        {
            var instance = Object.Instantiate(_prefab, _poolRoot);
            instance.gameObject.SetActive(false);
            instance.enabled = false;
            return instance;
        }

        private static void OnGet(CubeView cube)
        {
            if (cube == false)
                return;

            cube.gameObject.SetActive(true);
        }

        private void OnReleaseInternal(CubeView cube)
        {
            if (cube == false)
                return;

            var rectTransform = cube.transform as RectTransform;
            if (rectTransform)
                rectTransform.DOKill();

            DOTween.Kill(cube.gameObject);
            DOTween.Kill(cube.transform);

            cube.enabled = false;
            cube.Setup(null, null);

            BottomBarDragVisualUtility.SetGraphicRaycasts(cube.gameObject, false);

            if (_poolRoot)
                cube.transform.SetParent(_poolRoot, worldPositionStays: false);

            ResetToPrefabDefaults(cube);

            cube.gameObject.SetActive(false);
        }

        private void ResetToPrefabDefaults(CubeView cube)
        {
            if (cube == false || _prefabRect == false)
                return;

            var rectTransform = (RectTransform)cube.transform;

            rectTransform.anchorMin = _prefabRect.anchorMin;
            rectTransform.anchorMax = _prefabRect.anchorMax;
            rectTransform.pivot = _prefabRect.pivot;
            rectTransform.sizeDelta = _prefabRect.sizeDelta;

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }

        private static void PrepareForSpawn(CubeView cube, Transform parent)
        {
            if (cube == false)
                return;

            cube.transform.SetParent(parent, worldPositionStays: false);
            cube.transform.SetAsLastSibling();

            var rectTransform = cube.transform as RectTransform;
            if (rectTransform == false)
                return;

            rectTransform!.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private static void OnDestroyPooled(CubeView cube)
        {
            if (cube)
                Object.Destroy(cube.gameObject);
        }

        public void Dispose()
        {
            _pool?.Clear();
            _pool = null;

            if (_poolRoot)
                Object.Destroy(_poolRoot.gameObject);

            _poolRoot = null;
            _prefab = null;
            _prefabRect = null;
        }
    }
}