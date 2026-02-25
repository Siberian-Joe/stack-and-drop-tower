using System.Linq;
using Game.Config.Contracts;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class CubeViewFactory : ICubeViewFactory
    {
        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly IGameConfigReader _config;

        public CubeViewFactory(
            IGameplayWindowContext gameplayWindow,
            IGameConfigReader config)
        {
            _gameplayWindow = gameplayWindow;
            _config = config;
        }

        public CubeView Create(ICubeColorDefinition definition, Transform parent)
        {
            var prefab = _gameplayWindow.BottomBarView
                ? _gameplayWindow.BottomBarView.CubePrefab
                : null;

            if (definition == null || parent == false || prefab == false)
                return null;

            var cube = Object.Instantiate(prefab, parent);
            cube.Bind(definition);
            return cube;
        }

        public CubeView CreateByColorId(string colorId, Transform parent)
        {
            if (string.IsNullOrWhiteSpace(colorId))
                return null;

            var colors = _config.Current.BottomBar.Colors;
            return (from definition in colors
                    where definition != null && definition.Id == colorId
                    select Create(definition, parent))
                .FirstOrDefault();
        }

        public CubeView Clone(CubeView source, Transform parent)
        {
            if (source == false || parent == false)
                return null;

            var clone = Object.Instantiate(source.gameObject, parent);
            return clone.GetComponent<CubeView>();
        }
    }
}