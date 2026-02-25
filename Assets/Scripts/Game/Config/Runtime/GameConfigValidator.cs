using System;
using System.Collections.Generic;
using Game.Config.Contracts;

namespace Game.Config.Runtime
{
    public sealed class GameConfigValidator : IGameConfigValidator
    {
        public void Validate(IGameConfigDefinition config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var bottom = config.BottomBar;

            if (bottom.Count <= 0)
                throw new ArgumentException("BottomBar.Count must be > 0");

            if (bottom.Colors == null || bottom.Colors.Count == 0)
                throw new ArgumentException("BottomBar.Colors must have at least one entry");

            var ids = new HashSet<string>(StringComparer.Ordinal);

            foreach (var definition in bottom.Colors)
            {
                if (definition == null)
                    throw new ArgumentException("BottomBar.Colors contains null");

                if (string.IsNullOrWhiteSpace(definition.Id))
                    throw new ArgumentException("CubeColorDefinition.Id is empty");

                if (!ids.Add(definition.Id))
                    throw new ArgumentException($"Duplicate cube color id: '{definition.Id}'");

                if (definition.Sprite == false)
                    throw new ArgumentException($"CubeColorDefinition.Sprite is null for id: '{definition.Id}'");
            }

            var cubeDrag = config.CubeDrag
                           ?? throw new ArgumentException("CubeDrag config is null");

            if (cubeDrag.MaxXOffsetFactor is <= 0f or > 1f)
                throw new ArgumentException("CubeDrag.MaxXOffsetFactor must be in range (0..1]");

            if (cubeDrag.TowerFallDuration <= 0f)
                throw new ArgumentException("CubeDrag.TowerFallDuration must be > 0");

            if (cubeDrag.HoleEllipsePadding is <= 0f or > 1f)
                throw new ArgumentException("CubeDrag.HoleEllipsePadding must be in range (0..1]");

            if (cubeDrag.HolePullDuration <= 0f)
                throw new ArgumentException("CubeDrag.HolePullDuration must be > 0");

            if (cubeDrag.HoleFallDuration <= 0f)
                throw new ArgumentException("CubeDrag.HoleFallDuration must be > 0");

            if (cubeDrag.HoleFallExtra < 0f)
                throw new ArgumentException("CubeDrag.HoleFallExtra must be >= 0");

            if (cubeDrag.FailFallDuration <= 0f)
                throw new ArgumentException("CubeDrag.FailFallDuration must be > 0");

            if (cubeDrag.FailFallExtra < 0f)
                throw new ArgumentException("CubeDrag.FailFallExtra must be >= 0");
        }
    }
}