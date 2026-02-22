using System;
using Game.Progress.Contracts;
using R3;

namespace Game.Progress.Runtime
{
    public sealed class ProgressStore : IProgressReader, IProgressWriter, IProgressChanged
    {
        private GameProgressSnapshot _current;
        private readonly Subject<Unit> _changed = new();

        public GameProgressSnapshot Current
            => _current ?? throw new InvalidOperationException("Progress is not loaded");

        public Observable<Unit> Changed => _changed;

        public void Set(GameProgressSnapshot progress)
        {
            _current = progress ?? throw new ArgumentNullException(nameof(progress));
            _changed.OnNext(Unit.Default);
        }

        public void Update(Func<GameProgressSnapshot, GameProgressSnapshot> mutator)
        {
            _current = mutator(Current);
            _changed.OnNext(Unit.Default);
        }
    }
}