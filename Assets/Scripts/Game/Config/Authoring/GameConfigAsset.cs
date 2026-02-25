using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DG.Tweening;
using Game.Config.Contracts;
using UnityEngine;

namespace Game.Config.Authoring
{
    [CreateAssetMenu(menuName = "Game/Config/GameConfig", fileName = "GameConfig")]
    public sealed class GameConfigAsset : ScriptableObject, IGameConfigDefinition
    {
        [SerializeField] private BottomBarConfigDefinition _bottomBar = new();
        [SerializeField] private CubeDragConfigDefinition _cubeDrag = new();

        public IBottomBarConfigDefinition BottomBar => _bottomBar;
        public ICubeDragConfigDefinition CubeDrag => _cubeDrag;

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
            [SerializeField] private Sprite _sprite;

            public string Id => _id;
            public Sprite Sprite => _sprite;
        }

        [Serializable]
        private sealed class CubeDragConfigDefinition : ICubeDragConfigDefinition
        {
            [Header("Tower")]
            [SerializeField, Range(0.1f, 1f)] private float _maxXOffsetFactor = 0.5f;
            [SerializeField, Min(0.01f)] private float _towerFallDuration = 0.25f;
            [SerializeField] private Ease _towerFallEase = Ease.OutQuad;

            [Header("Hole")]
            [SerializeField, Range(0.5f, 1f)] private float _holeEllipsePadding = 0.9f;
            [SerializeField, Min(0.01f)] private float _holePullDuration = 0.12f;
            [SerializeField] private Ease _holePullEase = Ease.OutQuad;
            [SerializeField, Min(0.01f)] private float _holeFallDuration = 0.25f;
            [SerializeField] private Ease _holeFallEase = Ease.InQuad;
            [SerializeField, Min(0f)] private float _holeFallExtra = 60f;

            [Header("Fail")]
            [SerializeField, Min(0.01f)] private float _failFallDuration = 0.35f;
            [SerializeField] private Ease _failFallEase = Ease.InQuad;
            [SerializeField, Min(0f)] private float _failFallExtra = 80f;

            public float MaxXOffsetFactor => _maxXOffsetFactor;

            public float TowerFallDuration => _towerFallDuration;
            public Ease TowerFallEase => _towerFallEase;

            public float HoleEllipsePadding => _holeEllipsePadding;
            public float HolePullDuration => _holePullDuration;
            public Ease HolePullEase => _holePullEase;
            public float HoleFallDuration => _holeFallDuration;
            public Ease HoleFallEase => _holeFallEase;
            public float HoleFallExtra => _holeFallExtra;

            public float FailFallDuration => _failFallDuration;
            public Ease FailFallEase => _failFallEase;
            public float FailFallExtra => _failFallExtra;
        }
    }
}