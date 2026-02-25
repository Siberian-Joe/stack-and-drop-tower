using System;
using Game.UI.Screens.Contracts;
using UnityEngine;

namespace Game.UI.Screens.Runtime
{
    public sealed class ScreenRootsRegistry : IScreenRoots, IScreenRootsRegistry
    {
        private IScreenRoots _instance;

        public bool IsInitialized => _instance != null;

        public void Set(IScreenRoots instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Validate(instance);

            if (_instance != null && _instance != instance)
                throw new InvalidOperationException("ScreenRoots is already initialized");

            _instance = instance;
        }

        public RectTransform WindowRoot => Require().WindowRoot;
        public RectTransform OverlayRoot => Require().OverlayRoot;
        public RectTransform CacheRoot => Require().CacheRoot;
        public Canvas Canvas => Require().Canvas;

        private IScreenRoots Require()
            => _instance ?? throw new InvalidOperationException(
                "ScreenRoots is not initialized yet. " +
                "Ensure SpawnPersistentScreenRootsTask runs before UI usage.");

        private static void Validate(IScreenRoots roots)
        {
            if (roots.WindowRoot == false)
                throw new InvalidOperationException("ScreenRoots.WindowRoot is null");

            if (roots.OverlayRoot == false)
                throw new InvalidOperationException("ScreenRoots.OverlayRoot is null");

            if (roots.CacheRoot == false)
                throw new InvalidOperationException("ScreenRoots.CacheRoot is null");

            if (roots.Canvas == false)
                throw new InvalidOperationException("ScreenRoots.Canvas is null");
        }
    }
}