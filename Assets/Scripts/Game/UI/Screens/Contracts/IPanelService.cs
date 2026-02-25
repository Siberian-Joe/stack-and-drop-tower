using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.UI.Screens.Contracts
{
    public interface IPanelService
    {
        UniTask<IScreenHandle<TView>> LoadAsync<TView>(CancellationToken token = default)
            where TView : ScreenView;

        UniTask<IScreenHandle<TView>> LoadAndOpenAsync<TView>(CancellationToken token = default)
            where TView : ScreenView;

        void Open<TView>() where TView : ScreenView;
        void Close<TView>() where TView : ScreenView;
        void CloseAllOverlays();

        bool IsLoaded<TView>() where TView : ScreenView;
        bool IsOpen<TView>() where TView : ScreenView;

        bool TryGetView<TView>(out TView view) where TView : ScreenView;
    }
}