using Game.Config.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class BottomBarView : MonoBehaviour
    {
        [SerializeField] private RectTransform _contentRoot;
        [SerializeField] private BottomBarCubeView _cubePrefab;

        public void Build(IBottomBarConfigDefinition config)
        {
            if (config == null)
            {
                Debug.LogError("BottomBar config is null");
                return;
            }

            if (_contentRoot == false)
            {
                Debug.LogError("ContentRoot is null");
                return;
            }

            if (_cubePrefab == false)
            {
                Debug.LogError("CubePrefab is null");
                return;
            }

            if (config.Colors == null || config.Colors.Count == 0)
            {
                Debug.LogError("No cube colors");
                return;
            }

            for (var i = _contentRoot.childCount - 1; i >= 0; i--)
                Destroy(_contentRoot.GetChild(i).gameObject);

            var colors = config.Colors;

            for (var i = 0; i < config.Count; i++)
            {
                var definition = colors[i % colors.Count];

                var cube = Instantiate(_cubePrefab, _contentRoot);
                cube.Bind(definition);
            }
        }
    }
}