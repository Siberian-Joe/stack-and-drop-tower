using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Startup.Contracts;

namespace Game.Startup.Runtime
{
    public sealed class StartupPipeline<TTask> where TTask : IStartupTask
    {
        private readonly IReadOnlyList<TTask> _tasks;

        public StartupPipeline(IEnumerable<TTask> tasks) =>
            _tasks = tasks
                .OrderBy(startupTask => startupTask.Order)
                .ToArray();

        public async UniTask RunAsync(CancellationToken token)
        {
            foreach (var task in _tasks)
            {
                token.ThrowIfCancellationRequested();
                await task.ExecuteAsync(token);
            }
        }
    }
}