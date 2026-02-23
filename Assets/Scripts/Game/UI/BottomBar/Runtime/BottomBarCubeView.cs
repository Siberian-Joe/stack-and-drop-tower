using Game.Config.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class BottomBarCubeView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public string ColorId { get; private set; }

        public void Bind(ICubeColorDefinition definition)
        {
            ColorId = definition.Id;

            if (_image)
                _image.sprite = definition.Sprite;
        }
    }
}