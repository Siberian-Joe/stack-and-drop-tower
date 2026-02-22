using System;
using UnityEngine;

namespace Game.Progress.Data
{
    [Serializable]
    public sealed class CubePlacementData
    {
        public string colorId;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }
}