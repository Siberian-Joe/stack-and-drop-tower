using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime.Views;
using UnityEngine;

namespace Game.UI.Screens.Runtime.Definitions
{
    [CreateAssetMenu(
        menuName = "Game/UI/Screens/Definitions/Action Info Overlay",
        fileName = "ActionInfoOverlayDefinition")]
    public sealed class ActionInfoOverlayDefinition : ScreenDefinition<ActionInfoOverlayView>
    {
    }
}