using System;
using Cysharp.Threading.Tasks;
using Game.Lifetime.Contracts;
using Game.Startup.Contracts;
using R3;
using Zenject;

namespace Game.Startup.Runtime
{
    public sealed class StartupRunner<TStartup, TLifetime> : IInitializable, IDisposable
        where TStartup : IStartup
        where TLifetime : ILifetime
    {
        public ReadOnlyReactiveProperty<StartupState> State => _state;

        private readonly TStartup _startup;
        private readonly TLifetime _lifetime;
        private readonly ReactiveProperty<StartupState> _state = new(StartupState.NotStarted);

        public StartupRunner(TStartup startup, TLifetime lifetime)
        {
            _startup = startup;
            _lifetime = lifetime;
        }

        public void Initialize() => RunAsync().Forget();

        private async UniTaskVoid RunAsync()
        {
            _state.Value = StartupState.Running;

            try
            {
                await _startup.RunAsync(_lifetime.Token);
                _state.Value = StartupState.Succeeded;
            }
            catch (OperationCanceledException)
            {
                _state.Value = StartupState.Cancelled;
            }
            catch
            {
                _state.Value = StartupState.Failed;
            }
        }

        public void Dispose() => _state.Dispose();
    }
}