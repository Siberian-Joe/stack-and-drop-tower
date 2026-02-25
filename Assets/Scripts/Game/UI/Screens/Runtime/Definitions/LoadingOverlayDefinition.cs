using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime.Views;
using UnityEngine;

namespace Game.UI.Screens.Runtime.Definitions
{
    [CreateAssetMenu(
        menuName = "Game/UI/Screens/Definitions/Loading Overlay",
        fileName = "LoadingOverlayDefinition")]
    public sealed class LoadingOverlayDefinition : ScreenDefinition<LoadingOverlayView>
    {
    }
}