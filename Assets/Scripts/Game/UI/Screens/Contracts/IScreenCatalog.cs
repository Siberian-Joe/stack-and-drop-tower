using UnityEngine.AddressableAssets;

namespace Game.UI.Screens.Contracts
{
    public interface IScreenCatalog
    {
        AssetReferenceGameObject Get<TView>() where TView : ScreenView;
    }
}