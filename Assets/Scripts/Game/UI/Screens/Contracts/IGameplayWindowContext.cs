using DG.Tweening;
using Game.UI.BottomBar.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.Contracts
{
    public interface IGameplayWindowContext
    {
        BottomBarView BottomBarView { get; }

        ScrollRect ScrollRect { get; }

        RectTransform DragLayer { get; }

        RectTransform TowerRoot { get; }

        RectTransform HoleArea { get; }
        RectTransform HoleMaskRoot { get; }
        RectTransform HoleMouth { get; }

        Camera UiCamera { get; }

        float MaxXOffsetFactor { get; }

        float FallDuration { get; }
        Ease FallEase { get; }

        float HoleEllipsePadding { get; }
        float HolePullDuration { get; }
        Ease HolePullEase { get; }

        float HoleFallDuration { get; }
        Ease HoleFallEase { get; }
        float HoleFallExtra { get; }

        float FailFallDuration { get; }
        Ease FailFallEase { get; }
        float FailFallExtra { get; }
    }
}