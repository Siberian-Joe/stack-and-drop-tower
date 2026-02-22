using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Startup.Contracts;

namespace Game.Startup.Runtime
{
    public abstract class StartupBase<TTask> : IStartup where TTask : IStartupTask
    {
        private readonly StartupPipeline<TTask> _pipeline;

        protected StartupBase(StartupPipeline<TTask> pipeline) => _pipeline = pipeline;

        public virtual UniTask RunAsync(CancellationToken token) => _pipeline.RunAsync(token);
    }
}