using UnityEngine;

namespace Game.Progress.Runtime
{
    public readonly struct CubePlacementSnapshot
    {
        public readonly string ColorId;
        public readonly Vector3 LocalPosition;
        public readonly Quaternion LocalRotation;

        public CubePlacementSnapshot(string colorId, Vector3 localPosition, Quaternion localRotation)
        {
            ColorId = colorId ?? string.Empty;
            LocalPosition = localPosition;
            LocalRotation = localRotation;
        }
    }
}