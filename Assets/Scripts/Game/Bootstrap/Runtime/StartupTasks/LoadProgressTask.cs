using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Progress.Contracts;
using Game.Startup.Contracts;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class LoadProgressTask : IAppStartupTask
    {
        public string Name => "Load Progress";
        public int Order => 100;

        private readonly IProgressRepository _repository;
        private readonly IProgressWriter _writer;

        public LoadProgressTask(
            IProgressRepository repository,
            IProgressWriter writer)
        {
            _repository = repository;
            _writer = writer;
        }

        public async UniTask ExecuteAsync(CancellationToken token)
        {
            var progress = await _repository.LoadAsync(token);
            _writer.Set(progress);
        }
    }
}