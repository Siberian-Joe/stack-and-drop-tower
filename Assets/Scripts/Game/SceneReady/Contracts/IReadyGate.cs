using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.SceneReady.Contracts
{
    public interface IReadyGate
    {
        bool IsReady { get; }
        UniTask WaitReadyAsync(CancellationToken token);
        void Open();
    }
}