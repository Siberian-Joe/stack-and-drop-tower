using Game.Progress.Runtime;
using R3;

namespace Game.Progress.Contracts
{
    public interface ITowerProgressState
    {
        Observable<Unit> Changed { get; }

        TowerProgressSnapshot Snapshot { get; }
    }
}