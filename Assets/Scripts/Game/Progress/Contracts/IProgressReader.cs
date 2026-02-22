using Game.Progress.Runtime;

namespace Game.Progress.Contracts
{
    public interface IProgressReader
    {
        GameProgressSnapshot Current { get; }
    }
}