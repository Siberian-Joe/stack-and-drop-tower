using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.UI.Screens.Contracts;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.UI.Screens.Runtime
{
    public sealed class PanelService : IPanelService, IDisposable
    {
        private sealed class ScreenState
        {
            public Type ViewType;
            public GameObject Instance;
            public ScreenView View;
            public bool IsOpen;
        }

        private readonly IInstantiator _instantiator;
        private readonly IScreenRoots _roots;
        private readonly IScreenPrefabProvider _prefabs;

        private readonly Dictionary<Type, ScreenState> _states = new();
        private readonly Dictionary<Type, IScreenHandle> _handles = new();

        private readonly SemaphoreSlim _loadMutex = new(1, 1);
        private Type _activeWindowType;

        public PanelService(
            IInstantiator instantiator,
            IScreenRoots roots,
            IScreenPrefabProvider prefabs)
        {
            _instantiator = instantiator ?? throw new ArgumentNullException(nameof(instantiator));
            _roots = roots ?? throw new ArgumentNullException(nameof(roots));
            _prefabs = prefabs ?? throw new ArgumentNullException(nameof(prefabs));
        }

        public async UniTask<IScreenHandle<TView>> LoadAsync<TView>(CancellationToken token = default)
            where TView : ScreenView
        {
            var handle = GetOrCreateHandle<TView>();

            await _loadMutex.WaitAsync(token);
            try
            {
                await GetOrCreateStateAsync<TView>(token);
                return handle;
            }
            finally
            {
                _loadMutex.Release();
            }
        }

        public async UniTask<IScreenHandle<TView>> LoadAndOpenAsync<TView>(CancellationToken token = default)
            where TView : ScreenView
        {
            var handle = await LoadAsync<TView>(token);
            Open<TView>();
            return handle;
        }

        public void Open<TView>() where TView : ScreenView
        {
            var type = typeof(TView);

            if (!_states.TryGetValue(type, out var state))
                throw new InvalidOperationException($"Screen is not loaded. Call LoadAsync first: {type.FullName}");

            if (IsWindowType(type))
            {
                if (_activeWindowType != null && _activeWindowType != type)
                    CloseInternal(_activeWindowType);

                OpenInternal(state);
                _activeWindowType = type;
                return;
            }

            if (IsOverlayType(type) == false)
                throw new InvalidOperationException($"Unsupported screen type: {type.FullName}");

            OpenInternal(state);
        }

        public void Close<TView>() where TView : ScreenView =>
            CloseInternal(typeof(TView));

        public void CloseAllOverlays()
        {
            foreach (var type in _states.Keys.ToArray())
            {
                if (!IsOverlayType(type))
                    continue;

                CloseInternal(type);
            }
        }

        public bool IsLoaded<TView>() where TView : ScreenView =>
            IsLoaded(typeof(TView));

        public bool IsOpen<TView>() where TView : ScreenView =>
            IsOpen(typeof(TView));

        public bool TryGetHandle<TView>(out IScreenHandle<TView> handle) where TView : ScreenView
        {
            var type = typeof(TView);

            if (_handles.TryGetValue(type, out var existing))
            {
                handle = existing as IScreenHandle<TView>;
                return handle != null;
            }

            if (_states.ContainsKey(type))
            {
                handle = GetOrCreateHandle<TView>();
                return true;
            }

            handle = null;
            return false;
        }

        internal bool IsLoaded(Type viewType) =>
            _states.TryGetValue(viewType, out var state) && state.Instance && state.View;

        internal bool IsOpen(Type viewType) =>
            _states.TryGetValue(viewType, out var state) && state.IsOpen;

        internal bool TryGetView<TView>(out TView view) where TView : ScreenView
        {
            if (_states.TryGetValue(typeof(TView), out var state) &&
                state.View is TView typed &&
                state.Instance)
            {
                view = typed;
                return true;
            }

            view = null;
            return false;
        }

        private ScreenHandle<TView> GetOrCreateHandle<TView>() where TView : ScreenView
        {
            var type = typeof(TView);

            if (_handles.TryGetValue(type, out var existing))
                return (ScreenHandle<TView>)existing;

            var handle = new ScreenHandle<TView>(this);
            _handles.Add(type, handle);
            return handle;
        }

        private async UniTask<ScreenState> GetOrCreateStateAsync<TView>(CancellationToken token)
            where TView : ScreenView
        {
            var viewType = typeof(TView);

            if (_states.TryGetValue(viewType, out var existing))
                return existing;

            var prefab = await _prefabs.LoadPrefabAsync<TView>(token);
            if (prefab == false)
                throw new InvalidOperationException($"Prefab is null for screen: {viewType.FullName}");

            await UniTask.SwitchToMainThread(token);

            var instance = _instantiator.InstantiatePrefab(prefab, _roots.CacheRoot);
            if (instance == false)
                throw new InvalidOperationException($"Failed to instantiate screen prefab: {viewType.FullName}");

            var component = instance.GetComponent(viewType);
            if (component == false)
            {
                Object.Destroy(instance);
                throw new InvalidOperationException(
                    $"Prefab does not contain requested screen component: {viewType.FullName}");
            }

            if (component is not ScreenView screenView)
            {
                Object.Destroy(instance);
                throw new InvalidOperationException(
                    $"Screen component must inherit {nameof(ScreenView)}: {viewType.FullName}");
            }

            if (!IsWindowType(viewType) && !IsOverlayType(viewType))
            {
                Object.Destroy(instance);
                throw new InvalidOperationException(
                    $"Screen '{viewType.FullName}' must inherit {nameof(WindowScreenView)} or {nameof(OverlayScreenView)}");
            }

            instance.SetActive(false);
            MoveToParent(instance.transform, _roots.CacheRoot);

            var state = new ScreenState
            {
                ViewType = viewType,
                Instance = instance,
                View = screenView,
                IsOpen = false
            };

            _states.Add(viewType, state);
            return state;
        }

        private void OpenInternal(ScreenState state)
        {
            if (state == null || state.Instance == false || state.View == false)
                return;

            var parent = GetParentForType(state.ViewType);
            MoveToParent(state.Instance.transform, parent);

            state.View.OpenInternal();
            state.IsOpen = true;
        }

        private void CloseInternal(Type viewType)
        {
            if (viewType == null)
                return;

            if (!_states.TryGetValue(viewType, out var state))
                return;

            if (state.Instance == false || state.View == false)
                return;

            if (!state.IsOpen && state.Instance.activeSelf == false)
                return;

            state.View.CloseInternal();
            MoveToParent(state.Instance.transform, _roots.CacheRoot);
            state.IsOpen = false;

            if (_activeWindowType == viewType)
                _activeWindowType = null;
        }

        private Transform GetParentForType(Type viewType)
        {
            if (IsWindowType(viewType))
                return _roots.WindowRoot;

            return IsOverlayType(viewType)
                ? _roots.OverlayRoot
                : throw new InvalidOperationException($"Unsupported screen type: {viewType.FullName}");
        }

        private static bool IsWindowType(Type viewType) =>
            typeof(WindowScreenView).IsAssignableFrom(viewType);

        private static bool IsOverlayType(Type viewType) =>
            typeof(OverlayScreenView).IsAssignableFrom(viewType);

        private static void MoveToParent(Transform transform, Transform parent)
        {
            if (transform == false || parent == false)
                return;

            transform.SetParent(parent, worldPositionStays: false);
            transform.SetAsLastSibling();
        }

        public void Dispose()
        {
            foreach (var state in _states.Values.Where(x => x != null))
            {
                if (state.Instance)
                    Object.Destroy(state.Instance);
            }

            _states.Clear();
            _handles.Clear();

            _loadMutex.Dispose();
        }
    }
}