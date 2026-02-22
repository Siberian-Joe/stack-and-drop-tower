using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Startup.Contracts
{
    public interface IStartup
    {
        UniTask RunAsync(CancellationToken token);
    }
}