using System.Threading;

namespace Game.Lifetime.Contracts
{
    public interface ILifetime
    {
        CancellationToken Token { get; }
    }
}