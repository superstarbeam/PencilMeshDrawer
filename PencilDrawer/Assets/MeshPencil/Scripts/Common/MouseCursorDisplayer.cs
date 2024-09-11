using UnityEngine;

namespace MeshPencil.Common.MouseInputListener
{   
    [RequireComponent(typeof(SpriteRenderer))]
    public class MouseCursorDisplayer : MonoBehaviour
    {
        [SerializeField] private InputListener _inputListener;

        [Space]
        [Header("Cursor scale limits")]
        [SerializeField] private float _cursorStartScale;
        [SerializeField] private float _cursorMaxScale;
        [SerializeField] private float _cursorMinScale;

        [Header("State sprites")]
        [SerializeField] private Sprite _paintCursorSprite;
        [SerializeField] private Sprite _eraseCursorSprite;

        private bool _isInitialized;
        private SpriteRenderer _spriteRenderer;

        public float DrawCircleRadius { get; private set; }

        public void Initialize(InputListener inputListener = null)
        {   
            if(_isInitialized)
                return;

            if(inputListener != null)
                _inputListener = inputListener;

            _spriteRenderer = GetComponent<SpriteRenderer>();

            SubscribeEvents();
            
           SetCursorScale(_cursorStartScale);

           _isInitialized = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void ScaleCursor(float scaleAmount)
        {
            bool isScaleUp = scaleAmount > 0;

            if (IsOutOfScaleLimits(transform.localScale, isScaleUp))
                return;

            transform.localScale += new Vector3(
                scaleAmount,
                scaleAmount,
                scaleAmount);

            UpdateCircleRadius();
        }

        public void SetCursorScale(float scale)
        {
            transform.localScale = new Vector3(
                scale,
                scale,
                scale);

            UpdateCircleRadius();
        }

        public void Release()
        {
            UnsubscribeEvents();

            _inputListener = null;
        }

        private void MoveToPosition(Vector3 positionToMove)
        {
            transform.position = positionToMove;
        }

        private void UpdateCircleRadius()
        {
            DrawCircleRadius = transform.localScale.x;
        }

        private void ChangeCursorSprite(bool isEraseMode)
        {
            _spriteRenderer.sprite = isEraseMode ? _eraseCursorSprite : _paintCursorSprite;
        }

        private bool IsOutOfScaleLimits(Vector3 localScale, bool isScaleUp)
        {
            return isScaleUp ? localScale.x > _cursorMaxScale : localScale.x < _cursorMinScale;
        }

        private void SubscribeEvents()
        {
            if (_inputListener == null)
                return;

            _inputListener.Scaling += ScaleCursor;
            _inputListener.CursorPositionChange += MoveToPosition;
            _inputListener.ErasingModeChange += ChangeCursorSprite;
        }

        private void UnsubscribeEvents()
        {
            _inputListener.Scaling -= ScaleCursor;
            _inputListener.CursorPositionChange -= MoveToPosition;
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}
