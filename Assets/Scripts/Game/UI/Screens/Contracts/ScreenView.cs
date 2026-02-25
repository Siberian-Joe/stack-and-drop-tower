using UnityEngine;

namespace Game.UI.Screens.Contracts
{
    public abstract class ScreenView : MonoBehaviour
    {
        public bool IsOpen { get; private set; }

        internal void OpenInternal()
        {
            if (IsOpen)
                return;

            gameObject.SetActive(true);
            IsOpen = true;
            OnOpened();
        }

        internal void CloseInternal()
        {
            if (!IsOpen && gameObject.activeSelf == false)
                return;

            OnClosed();
            IsOpen = false;
            gameObject.SetActive(false);
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual void OnClosed()
        {
        }
    }
}