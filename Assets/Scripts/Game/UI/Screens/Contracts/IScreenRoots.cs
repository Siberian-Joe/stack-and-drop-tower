using UnityEngine;

namespace Game.UI.Screens.Contracts
{
    public interface IScreenRoots
    {
        RectTransform WindowRoot { get; }
        RectTransform OverlayRoot { get; }
        RectTransform CacheRoot { get; }
        Canvas Canvas { get; }
    }
}