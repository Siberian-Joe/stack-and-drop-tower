using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Game.Config.Contracts;
using UnityEngine;

namespace Game.Config.Authoring
{
    [CreateAssetMenu(menuName = "Game/Config/GameConfig", fileName = "GameConfig")]
    public sealed class GameConfigAsset : ScriptableObject, IGameConfigDefinition
    {
        [SerializeField] private BottomBarConfigDefinition bottomBar = new();

        public IBottomBarConfigDefinition BottomBar => bottomBar;

        [Serializable]
        private sealed class BottomBarConfigDefinition : IBottomBarConfigDefinition
        {
            [SerializeField] private int _count = 20;
            [SerializeField] private List<CubeColorDefinition> _colors = new();

            [NonSerialized] private ReadOnlyCollection<CubeColorDefinition> _colorsView;

            public int Count => _count;

            public IReadOnlyList<ICubeColorDefinition> Colors
                => _colorsView ??= _colors.AsReadOnly();
        }

        [Serializable]
        private sealed class CubeColorDefinition : ICubeColorDefinition
        {
            [SerializeField] private string _id;
            [SerializeField] private Color32 _color = new(255, 255, 255, 255);
            [SerializeField] private string _localizationKey;

            public string Id => _id;
            public Color32 Color => _color;
            public string localizationKey => _localizationKey;
        }
    }
}