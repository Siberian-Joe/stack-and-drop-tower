using System;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Screens.Contracts;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.UI.Screens.Runtime
{
    [CreateAssetMenu(menuName = "Game/UI/Screens/Screen Catalog", fileName = "ScreenCatalog")]
    public sealed class ScreenCatalog : ScriptableObject, IScreenCatalog
    {
        [SerializeField] private ScreenDefinition[] _screens;

        private Dictionary<Type, ScreenDefinition> _map;

        public AssetReferenceGameObject Get<TView>() where TView : ScreenView
        {
            var viewType = typeof(TView);

            if (viewType == null)
                throw new ArgumentNullException(nameof(viewType));

            EnsureMap();

            return _map.TryGetValue(viewType, out var definition) == false
                ? throw new InvalidOperationException($"Screen type is not registered: {viewType.FullName}")
                : definition.Prefab;
        }

        private void EnsureMap()
        {
            if (_map != null)
                return;

            _map = new Dictionary<Type, ScreenDefinition>();

            foreach (var definition in _screens.Where(x => x))
            {
                definition.Validate();

                var type = definition.ViewType;
                if (!_map.TryAdd(type, definition))
                    throw new InvalidOperationException($"Duplicate screen registration for type: {type.FullName}");
            }
        }
    }
}