using System;
using System.Collections.Generic;
using Game.Config.Contracts;

namespace Game.Config.Runtime
{
    public sealed class GameConfigValidator : IGameConfigValidator
    {
        public void Validate(IGameConfigDefinition config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

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
        }
    }
}