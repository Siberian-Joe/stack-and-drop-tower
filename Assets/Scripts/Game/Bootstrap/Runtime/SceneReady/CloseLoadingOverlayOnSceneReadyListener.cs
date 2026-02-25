using System.Threading;
using Cysharp.Threading.Tasks;
using Game.SceneReady.Contracts;
using Game.UI.Screens.Contracts;

namespace Game.Bootstrap.Runtime.SceneReady
{
    public sealed class CloseLoadingOverlayOnSceneReadyListener : ISceneReadyListener
    {
        public int Order => -10_000;

        private readonly ISceneTransitionCurtain _transitionCurtain;

        public CloseLoadingOverlayOnSceneReadyListener(ISceneTransitionCurtain transitionCurtain) =>
            _transitionCurtain = transitionCurtain;

        public UniTask OnSceneReadyAsync(CancellationToken token)
        {
            _transitionCurtain.Hide();
            return UniTask.CompletedTask;
        }
    }
}