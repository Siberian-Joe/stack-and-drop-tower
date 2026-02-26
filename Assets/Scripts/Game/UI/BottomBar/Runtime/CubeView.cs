using Game.Config.Contracts;
using Game.UI.BottomBar.Contracts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class CubeView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _image;

        public string ColorId { get; private set; }
        public Sprite Sprite { get; private set; }

        private ScrollRect _scrollRect;
        private ICubeDragInteractor _dragInteractor;

        private Vector2 _startPos;
        private Mode _mode = Mode.None;

        private const float DecideThreshold = 12f;

        private enum Mode
        {
            None,
            Scrolling,
            Dragging
        }

        public void Bind(ICubeColorDefinition def)
        {
            ColorId = def.Id;
            Sprite = def.Sprite;

            if (_image)
                _image.sprite = def.Sprite;
        }

        public void Setup(ICubeDragInteractor dragInteractor, ScrollRect scrollRect = null)
        {
            _dragInteractor = dragInteractor;
            _scrollRect = scrollRect;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_dragInteractor == null)
                return;

            _startPos = eventData.position;

            if (_scrollRect == false)
            {
                _mode = Mode.Dragging;
                _dragInteractor.BeginDrag(this, eventData.pointerId, eventData.position);
                return;
            }

            _mode = Mode.None;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragInteractor == null)
                return;

            if (_scrollRect == false)
            {
                _dragInteractor.Move(eventData.position);
                return;
            }

            var delta = eventData.position - _startPos;

            if (_mode == Mode.None)
            {
                if (delta.magnitude < DecideThreshold)
                    return;

                _mode = Mathf.Abs(delta.y) > Mathf.Abs(delta.x)
                    ? Mode.Dragging
                    : Mode.Scrolling;

                if (_mode == Mode.Dragging)
                {
                    _dragInteractor.BeginDrag(this, eventData.pointerId, eventData.position);
                    return;
                }

                _scrollRect.OnBeginDrag(eventData);
            }

            if (_mode == Mode.Dragging)
            {
                _dragInteractor.Move(eventData.position);
                return;
            }

            _scrollRect.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragInteractor == null)
                return;

            if (_scrollRect == false)
            {
                if (_mode == Mode.Dragging)
                    _dragInteractor.EndDrag(eventData.pointerId);

                _mode = Mode.None;
                return;
            }

            switch (_mode)
            {
                case Mode.Dragging:
                    _dragInteractor.EndDrag(eventData.pointerId);
                    break;
                case Mode.Scrolling:
                    _scrollRect.OnEndDrag(eventData);
                    break;
            }

            _mode = Mode.None;
        }
    }
}