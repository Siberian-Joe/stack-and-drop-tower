using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Lifetime.Contracts;
using Game.Progress.Contracts;
using Game.SceneReady.Contracts;
using R3;

namespace Game.Progress.Runtime.Listeners
{
    public sealed class AutoSaveListener : ISceneReadyListener, IDisposable
    {
        public int Order => 10_000;

        private readonly IProgressChanged _changed;
        private readonly IProgressReader _reader;
        private readonly IProgressRepository _repo;
        private readonly IAppLifetime _appLifetime;

        private readonly CompositeDisposable _disposables = new();
        private CancellationTokenSource _saveCts;

        public AutoSaveListener(IProgressChanged changed, IProgressReader reader, IProgressRepository repo,
            IAppLifetime appLifetime)
        {
            _changed = changed;
            _reader = reader;
            _repo = repo;
            _appLifetime = appLifetime;
        }

        public UniTask OnSceneReadyAsync(CancellationToken token)
        {
            _changed.Changed
                .Subscribe(_ => RequestSaveDebounced(300))
                .AddTo(_disposables);

            return UniTask.CompletedTask;
        }

        private void RequestSaveDebounced(int ms)
        {
            _saveCts?.Cancel();
            _saveCts?.Dispose();

            _saveCts = CancellationTokenSource.CreateLinkedTokenSource(_appLifetime.Token);
            SaveLaterAsync(ms, _saveCts.Token).Forget();
        }

        private async UniTaskVoid SaveLaterAsync(int ms, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(ms, cancellationToken: token);
                await _repo.SaveAsync(_reader.Current, token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _saveCts?.Cancel();
            _saveCts?.Dispose();
        }
    }
}