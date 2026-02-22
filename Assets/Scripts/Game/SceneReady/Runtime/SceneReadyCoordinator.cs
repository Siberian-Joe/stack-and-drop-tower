using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Lifetime.Contracts;
using Game.SceneReady.Contracts;
using UnityEngine;
using Zenject;

namespace Game.SceneReady.Runtime
{
    public sealed class SceneReadyCoordinator : IInitializable
    {
        private readonly ISceneReadyGate _gate;
        private readonly ISceneLifetime _lifetime;
        private readonly IReadOnlyList<ISceneReadyListener> _listeners;

        public SceneReadyCoordinator(
            ISceneReadyGate gate,
            ISceneLifetime lifetime,
            List<ISceneReadyListener> listeners)
        {
            _gate = gate;
            _lifetime = lifetime;
            _listeners = listeners;
        }

        public void Initialize() => RunAsync().Forget();

        private async UniTaskVoid RunAsync()
        {
            try
            {
                await _gate.WaitReadyAsync(_lifetime.Token);

                foreach (var listener in _listeners.OrderBy(listener => listener.Order))
                    await listener.OnSceneReadyAsync(_lifetime.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}