using Game.Config.Contracts;
using Game.UI.BottomBar.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class CubeViewFactory : ICubeViewFactory
    {
        private readonly BottomBarView _bottomBarView;
        private readonly IGameConfigReader _config;

        public CubeViewFactory(
            BottomBarView bottomBarView,
            IGameConfigReader config)
        {
            _bottomBarView = bottomBarView;
            _config = config;
        }

        public CubeView Create(ICubeColorDefinition definition, Transform parent)
        {
            if (definition == null || parent == null || _bottomBarView.CubePrefab == null)
                return null;

            var cube = Object.Instantiate(_bottomBarView.CubePrefab, parent);
            cube.Bind(definition);
            return cube;
        }

        public CubeView CreateByColorId(string colorId, Transform parent)
        {
            if (string.IsNullOrWhiteSpace(colorId))
                return null;

            var colors = _config.Current.BottomBar.Colors;
            for (var i = 0; i < colors.Count; i++)
            {
                var def = colors[i];
                if (def != null && def.Id == colorId)
                    return Create(def, parent);
            }

            return null;
        }

        public CubeView Clone(CubeView source, Transform parent)
        {
            if (source == null || parent == null)
                return null;

            var clone = Object.Instantiate(source.gameObject, parent);
            return clone.GetComponent<CubeView>();
        }
    }
}