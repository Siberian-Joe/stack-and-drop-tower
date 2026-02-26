using Game.Config.Contracts;
using Game.UI.BottomBar.Runtime;
using UnityEngine;

namespace Game.UI.BottomBar.Contracts
{
    public interface ICubeViewFactory
    {
        CubeView Create(ICubeColorDefinition definition, Transform parent);
        CubeView CreateByColorId(string colorId, Transform parent);
        CubeView Clone(CubeView source, Transform parent);

        void Release(CubeView cube);
    }
}