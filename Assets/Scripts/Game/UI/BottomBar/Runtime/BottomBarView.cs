using Game.Config.Contracts;
using Game.UI.BottomBar.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class BottomBarView : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _contentRoot;
        [SerializeField] private CubeView _cubePrefab;

        public ScrollRect ScrollRect => _scrollRect;
        public CubeView CubePrefab => _cubePrefab;

        public void Build(
            IBottomBarConfigDefinition config,
            ICubeDragInteractor dragInteractor,
            ICubeViewFactory cubeFactory)
        {
            for (var i = _contentRoot.childCount - 1; i >= 0; i--)
            {
                var child = _contentRoot.GetChild(i);

                if (cubeFactory != null && child.TryGetComponent<CubeView>(out var cube))
                {
                    cubeFactory.Release(cube);
                    continue;
                }

                Destroy(child.gameObject);
            }

            var colors = config.Colors;
            if (colors == null || colors.Count == 0)
                return;

            for (var i = 0; i < config.Count; i++)
            {
                var def = colors[i % colors.Count];

                var cube = cubeFactory != null
                    ? cubeFactory.Create(def, _contentRoot)
                    : Instantiate(_cubePrefab, _contentRoot);

                if (cube == false)
                    continue;

                if (cubeFactory == null)
                    cube.Bind(def);

                cube.Setup(dragInteractor, _scrollRect);
            }
        }
    }
}