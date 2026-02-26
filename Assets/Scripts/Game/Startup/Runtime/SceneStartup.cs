using System.Threading;
using Cysharp.Threading.Tasks;
using Game.SceneReady.Contracts;
using Game.Startup.Contracts;

namespace Game.Startup.Runtime
{
    public sealed class SceneStartup : StartupBase<ISceneStartupTask>, ISceneStartup
    {
        private readonly ISceneReadyGate _sceneGate;
        private readonly IAppReadyGate _appGate;

        public SceneStartup(
            StartupPipeline<ISceneStartupTask> pipeline,
            ISceneReadyGate sceneGate,
            IAppReadyGate appGate) : base(pipeline)
        {
            _sceneGate = sceneGate;
            _appGate = appGate;
        }

        public override async UniTask RunAsync(CancellationToken token)
        {
            await _appGate.WaitReadyAsync(token);

            await base.RunAsync(token);

            await UniTask.SwitchToMainThread(token);
            _sceneGate.Open();
        }
    }
}