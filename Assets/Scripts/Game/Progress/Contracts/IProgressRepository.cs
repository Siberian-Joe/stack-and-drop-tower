using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Progress.Runtime;

namespace Game.Progress.Contracts
{
    public interface IProgressRepository
    {
        UniTask<GameProgressSnapshot> LoadAsync(CancellationToken token);
        UniTask SaveAsync(GameProgressSnapshot progress, CancellationToken token);
    }
}