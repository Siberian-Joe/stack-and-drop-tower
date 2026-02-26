using System.Threading;
using Cysharp.Threading.Tasks;
using Game.SceneReady.Contracts;
using Game.Startup.Contracts;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class OpenAppReadyGateTask : IAppStartupTask
    {
        public string Name => "Open App Ready Gate";
        public int Order => 900;

        private readonly IAppReadyGate _gate;

        public OpenAppReadyGateTask(IAppReadyGate gate) => _gate = gate;

        public UniTask ExecuteAsync(CancellationToken token)
        {
            _gate.Open();
            return UniTask.CompletedTask;
        }
    }
}