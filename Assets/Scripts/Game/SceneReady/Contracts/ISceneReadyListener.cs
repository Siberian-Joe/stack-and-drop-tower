using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.SceneReady.Contracts
{
    public interface ISceneReadyListener
    {
        int Order { get; }
        UniTask OnSceneReadyAsync(CancellationToken token);
    }
}