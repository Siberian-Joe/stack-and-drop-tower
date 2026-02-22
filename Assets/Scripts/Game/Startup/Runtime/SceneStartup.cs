using System.Threading;
using Cysharp.Threading.Tasks;
using Game.SceneReady.Contracts;
using Game.Startup.Contracts;

namespace Game.Startup.Runtime
{
    public sealed class SceneStartup : StartupBase<ISceneStartupTask>, ISceneStartup
    {
        private readonly ISceneReadyGate _gate;

        public SceneStartup(
            StartupPipeline<ISceneStartupTask> pipeline,
            ISceneReadyGate gate) : base(pipeline) =>
            _gate = gate;

        public override async UniTask RunAsync(CancellationToken token)
        {
            await base.RunAsync(token);
            await UniTask.SwitchToMainThread(token);
            _gate.Open();
        }
    }
}