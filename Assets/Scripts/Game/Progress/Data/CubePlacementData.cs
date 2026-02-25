using System;
using UnityEngine;

namespace Game.Progress.Data
{
    [Serializable]
    public sealed class CubePlacementData
    {
        public string ColorId;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
    }
}