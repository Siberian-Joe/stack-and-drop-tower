using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime.Views;
using UnityEngine;

namespace Game.UI.Screens.Runtime.Definitions
{
    [CreateAssetMenu(
        menuName = "Game/UI/Screens/Definitions/Gameplay Window",
        fileName = "GameplayWindowDefinition")]
    public sealed class GameplayWindowDefinition : ScreenDefinition<GameplayWindowView>
    {
    }
}