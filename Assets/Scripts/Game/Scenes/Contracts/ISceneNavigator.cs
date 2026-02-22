using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scenes.Contracts
{
    public interface ISceneNavigator
    {
        UniTask SwitchToAsync(SceneId target, CancellationToken token);
    }
}