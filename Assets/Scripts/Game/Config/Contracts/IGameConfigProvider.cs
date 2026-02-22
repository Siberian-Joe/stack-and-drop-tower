using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Config.Contracts
{
    public interface IGameConfigProvider
    {
        UniTask<IGameConfigDefinition> LoadAsync(CancellationToken token);
    }
}