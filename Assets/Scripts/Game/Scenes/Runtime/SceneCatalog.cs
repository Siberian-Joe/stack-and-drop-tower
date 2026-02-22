using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scenes.Contracts;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scenes.Runtime
{
    [CreateAssetMenu(menuName = "Game/Scenes/SceneCatalog", fileName = "SceneCatalog")]
    public sealed class SceneCatalog : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public SceneId id;
            public AssetReference scene;
        }

        [SerializeField] private List<Entry> entries = new();

        public AssetReference Get(SceneId id)
            => entries.First(entry => entry.id == id).scene;
    }
}