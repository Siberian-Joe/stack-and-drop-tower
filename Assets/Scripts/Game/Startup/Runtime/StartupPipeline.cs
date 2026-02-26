using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Startup.Contracts;
using UnityEngine;

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
                try
                {
                    await task.ExecuteAsync(token);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }
}