using System;
using Game.Progress.Contracts;
using R3;
using Zenject;

namespace Game.Progress.Runtime.Listeners
{
    public sealed class TowerProgressToStoreBridge : IInitializable, IDisposable
    {
        private readonly ITowerProgressState _towerProgress;
        private readonly IProgressWriter _progressWriter;
        private readonly CompositeDisposable _disposables = new();

        public TowerProgressToStoreBridge(
            ITowerProgressState towerProgress,
            IProgressWriter progressWriter)
        {
            _towerProgress = towerProgress;
            _progressWriter = progressWriter;
        }

        public void Initialize()
        {
            _towerProgress.Changed
                .Subscribe(_ => SyncTower())
                .AddTo(_disposables);
        }

        private void SyncTower() => _progressWriter.Update(current => current.WithTower(_towerProgress.Snapshot));

        public void Dispose() => _disposables.Dispose();
    }
}