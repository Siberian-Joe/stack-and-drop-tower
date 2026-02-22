using System;
using Game.Config.Contracts;

namespace Game.Config.Runtime
{
    public sealed class GameConfigValidator : IGameConfigValidator
    {
        public void Validate(IGameConfigDefinition config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.BottomBar.Count <= 0)
                throw new ArgumentException("BottomBar count must be greater than 0");

            if (config.BottomBar.Colors.Count == 0)
                throw new ArgumentException("BottomBar must have at least one color");
        }
    }
}