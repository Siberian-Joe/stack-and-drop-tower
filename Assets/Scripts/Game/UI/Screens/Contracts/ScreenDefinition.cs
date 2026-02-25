using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.UI.Screens.Contracts
{
    public abstract class ScreenDefinition<TView> : ScreenDefinition
        where TView : ScreenView
    {
        public sealed override Type ViewType => typeof(TView);
    }

    public abstract class ScreenDefinition : ScriptableObject
    {
        [SerializeField] private AssetReferenceGameObject _prefab;

        public AssetReferenceGameObject Prefab => _prefab;

        public abstract Type ViewType { get; }

        public void Validate()
        {
            if (_prefab == null)
                throw new InvalidOperationException($"{name}: Prefab is null");

            var type = ViewType;
            if (type == null)
                throw new InvalidOperationException($"{name}: ViewType is null");

            if (!typeof(ScreenView).IsAssignableFrom(type))
                throw new InvalidOperationException($"{name}: {type.FullName} must inherit {nameof(ScreenView)}");

            if (!typeof(WindowScreenView).IsAssignableFrom(type) &&
                !typeof(OverlayScreenView).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    $"{name}: {type.FullName} must inherit {nameof(WindowScreenView)} or {nameof(OverlayScreenView)}");
            }
        }
    }
}