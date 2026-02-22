using R3;

namespace Game.Progress.Contracts
{
    public interface IProgressChanged
    {
        Observable<Unit> Changed { get; }
    }
}