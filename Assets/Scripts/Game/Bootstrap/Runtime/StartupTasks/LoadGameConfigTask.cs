using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Config.Contracts;
using Game.Startup.Contracts;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class LoadGameConfigTask : IAppStartupTask
    {
        public string Name => "Load Game Config";
        public int Order => 10;

        private readonly IGameConfigProvider _provider;
        private readonly IGameConfigValidator _validator;
        private readonly IGameConfigWriter _writer;

        public LoadGameConfigTask(
            IGameConfigProvider provider,
            IGameConfigValidator validator,
            IGameConfigWriter writer)
        {
            _provider = provider;
            _validator = validator;
            _writer = writer;
        }

        public async UniTask ExecuteAsync(CancellationToken token)
        {
            var config = await _provider.LoadAsync(token);
            _validator.Validate(config);
            _writer.Set(config);
        }
    }
}