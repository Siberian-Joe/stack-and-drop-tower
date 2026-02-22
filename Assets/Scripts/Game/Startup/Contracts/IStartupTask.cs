using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Startup.Contracts
{
    public interface IStartupTask
    {
        string Name { get; }
        int Order { get; }
        UniTask ExecuteAsync(CancellationToken token);
    }
}