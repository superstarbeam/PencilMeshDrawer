using System;
using UnityEngine;

namespace MeshPencil.Drawers.PixelDrawer
{   
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpritePixel : MonoBehaviour
    {
        [SerializeField] private SpritePixelData _pixelData;

        private SpriteRenderer _spriteRenderer;

        private bool _isHighlighted;
        private bool _isPainted;

        private Vector2 _indexInArray;

        public Vector3 PointPosition { get; private set; }

        public Vector2 IndexInArray => _indexInArray;

        public bool IsPainted => _isPainted;

        public void Initialize(Vector2 indexInCanvasArray)
        {
            _indexInArray = indexInCanvasArray;
            PointPosition = transform.position;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetHighlighted(bool isHighlighted)
        {
            _isHighlighted = isHighlighted;
        }

        public void SetPainted(bool isPainted)
        {
            _isPainted = isPainted;

            if (isPainted)
            {
                SetPaintedView();
            }
            else
            {
                SetErasedView();
            }

        }

        public void UpdateView()
        {
            if (_isPainted) return;
            _spriteRenderer.color = _isHighlighted ?
                _pixelData.DetectedColor :
                _pixelData.UndetectedColor;
        }

        public void Reset()
        {
            _isPainted = false;
        }

        private void SetPaintedView()
        {
            _spriteRenderer.color = _pixelData.PaintedColor;
        }

        private void SetErasedView()
        {
            _spriteRenderer.color = _pixelData.ErasedColor;
        }
    }
}
