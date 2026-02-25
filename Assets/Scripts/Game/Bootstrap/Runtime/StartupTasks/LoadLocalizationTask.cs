using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Localization.Contracts;
using Game.Startup.Contracts;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class LoadLocalizationTask : IAppStartupTask
    {
        public string Name => "Load Localization";
        public int Order => 20;

        private readonly ILocalizationProvider _provider;
        private readonly ILocalizationWriter _writer;

        public LoadLocalizationTask(
            ILocalizationProvider provider,
            ILocalizationWriter writer)
        {
            _provider = provider;
            _writer = writer;
        }

        public async UniTask ExecuteAsync(CancellationToken token)
        {
            var definition = await _provider.LoadAsync(token);
            _writer.Set(definition);
        }
    }
}