using DG.Tweening;
using Game.UI.Screens.Contracts;
using TMPro;
using UnityEngine;

namespace Game.UI.Screens.Runtime.Views
{
    public sealed class ActionInfoOverlayView : OverlayScreenView
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;

        [Header("Animation")]
        [SerializeField, Min(0f)] private float _fadeInDuration = 0.08f;
        [SerializeField, Min(0f)] private float _showDuration = 1.1f;
        [SerializeField, Min(0f)] private float _fadeOutDuration = 0.18f;

        private Sequence _sequence;

        protected override void OnOpened() => ResetVisual();

        private void Awake() => ResetVisual();

        private void ResetVisual()
        {
            if (_canvasGroup == false)
                return;

            _sequence?.Kill();
            _canvasGroup.DOKill();

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Show(string message)
        {
            if (_text == false || string.IsNullOrWhiteSpace(message))
                return;

            _text.text = message;

            if (_canvasGroup == false)
                return;

            _sequence?.Kill();
            _canvasGroup.DOKill();

            _canvasGroup.alpha = 0f;

            _sequence = DOTween.Sequence()
                .SetTarget(this)
                .Append(_canvasGroup.DOFade(1f, _fadeInDuration))
                .AppendInterval(_showDuration)
                .Append(_canvasGroup.DOFade(0f, _fadeOutDuration))
                .SetLink(gameObject);
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
            _canvasGroup?.DOKill();
        }
    }
}