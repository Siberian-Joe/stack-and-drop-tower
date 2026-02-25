using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.UI.Screens.Contracts
{
    public interface ISceneTransitionCurtain
    {
        UniTask ShowAsync(CancellationToken token);
        void Hide();
    }
}