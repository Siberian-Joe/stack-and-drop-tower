using System;
using Game.Config.Contracts;

namespace Game.Config.Runtime
{
    public sealed class GameConfigStore : IGameConfigReader, IGameConfigWriter
    {
        private IGameConfigDefinition _current;

        public IGameConfigDefinition Current
            => _current ?? throw new InvalidOperationException("GameConfig is not loaded");

        public void Set(IGameConfigDefinition config) =>
            _current = config ?? throw new ArgumentNullException(nameof(config));
    }
}