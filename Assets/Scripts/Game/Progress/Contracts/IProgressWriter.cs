using System;
using Game.Progress.Runtime;

namespace Game.Progress.Contracts
{
    public interface IProgressWriter
    {
        void Set(GameProgressSnapshot progress);
        void Update(Func<GameProgressSnapshot, GameProgressSnapshot> mutator);
    }
}