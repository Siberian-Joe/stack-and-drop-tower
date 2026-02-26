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

        private bool _disposed;

        public StartupRunner(TStartup startup, TLifetime lifetime)
        {
            _startup = startup;
            _lifetime = lifetime;
        }

        public void Initialize() => RunAsync().Forget();

        private async UniTaskVoid RunAsync()
        {
            SetState(StartupState.Running);

            try
            {
                await _startup.RunAsync(_lifetime.Token);
                SetState(StartupState.Succeeded);
            }
            catch (OperationCanceledException)
            {
                SetState(StartupState.Cancelled);
            }
            catch
            {
                SetState(StartupState.Failed);
            }
        }

        private void SetState(StartupState state)
        {
            if (_disposed) return;

            try
            {
                _state.Value = state;
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _state.Dispose();
        }
    }
}