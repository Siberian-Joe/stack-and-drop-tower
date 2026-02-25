using DG.Tweening;

namespace Game.Config.Contracts
{
    public interface ICubeDragConfigDefinition
    {
        float MaxXOffsetFactor { get; }

        float TowerFallDuration { get; }
        Ease TowerFallEase { get; }

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