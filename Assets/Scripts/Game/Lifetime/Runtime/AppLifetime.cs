using System.Threading;
using Game.Lifetime.Contracts;

namespace Game.Lifetime.Runtime
{
    public sealed class AppLifetime : IAppLifetime
    {
        public CancellationToken Token { get; }
        public AppLifetime(CancellationToken token) => Token = token;
    }
}