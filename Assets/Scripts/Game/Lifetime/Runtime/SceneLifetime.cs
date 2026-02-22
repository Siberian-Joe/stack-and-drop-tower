using System.Threading;
using Game.Lifetime.Contracts;

namespace Game.Lifetime.Runtime
{
    public sealed class SceneLifetime : ISceneLifetime
    {
        public CancellationToken Token { get; }
        public SceneLifetime(CancellationToken token) => Token = token;
    }
}