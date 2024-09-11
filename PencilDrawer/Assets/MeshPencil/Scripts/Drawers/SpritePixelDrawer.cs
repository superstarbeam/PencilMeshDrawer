using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshPencil.Drawers.PixelDrawer
{
    public class SpritePixelDrawer : PixelDrawer
    {
        public override event Action<byte[,]> DrawFinished;
        public override event Action<string> DrawFinishFailed;

        [Tooltip("Single pixel prefab")]
        [SerializeField]
        private GameObject _pointPrefab;

        private List<SpritePixel> _pixels;

        private int _pixelsInRowAmount;
        private int _columnsAmount;

        private float _pixelSize;

        private bool _isDetectingNow;

        private GameObject pixelsRoot;

        public override void Initialize(PixelDrawerData data)
        {
            _pixelSize = data.PixelSize;
            _pixelsInRowAmount = data.PixelsInRowAmount;
            _columnsAmount = data.ColumnsAmount;

            RemoveAllPixels();
            
            _isDetectingNow = true;
            SpawnPixelObjects();
        }

        public override void SpawnPixelObjects()
        {
            _pixels = new List<SpritePixel>();

            Vector3 offsetedPosition = transform.position + new Vector3(_pixelSize/2, _pixelSize/2, 0);

            float yPosition = 0;

            pixelsRoot = Instantiate(new GameObject(), gameObject.transform.position, gameObject.transform.rotation);
            pixelsRoot.transform.parent = gameObject.transform;
            pixelsRoot.name = "Pixels";
            
            for (int column = 0; column < _columnsAmount; column++)
            {
                float xPosition = 0;

                for (int row = 0; row < _pixelsInRowAmount; row++)
                {
                    Vector3 pointPosition = new Vector3(
                        offsetedPosition.x + xPosition,
                        offsetedPosition.y + yPosition,
                        transform.position.z);

                    Vector3 pointScale = new Vector3(_pixelSize, _pixelSize, _pixelSize);

                    GameObject spawnedPointObject = Instantiate(_pointPrefab, pointPosition, Quaternion.identity);

                    SpritePixel objectToHighlight = spawnedPointObject.GetComponent<SpritePixel>();
                    objectToHighlight.Initialize(new Vector2(column, row));

                    spawnedPointObject.transform.localScale = pointScale;
                    spawnedPointObject.transform.parent = pixelsRoot.transform;

                    spawnedPointObject.layer = gameObject.layer;

                    _pixels.Add(objectToHighlight);

                    xPosition += _pixelSize;
                }

                yPosition += _pixelSize;
            }
        }

        public override void FinishDrawing()
        {
            _isDetectingNow = false;

            byte[,] data = CreateDataFromPixels();

            if (IsCanvasPainted(data))
            {
                DrawFinished?.Invoke(data);
            }
            else
            {
                DrawFinishFailed?.Invoke("Created null data");
            }
        }

        private bool IsCanvasPainted( byte[,] data)
        {
            bool isPainted = false;
            
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int k = 0; k < data.GetLength(1); k++)
                {
                    if (data[i, k] == 1)
                    {
                        isPainted = true;
                        break;
                    }
                }
                
                if(isPainted)
                    break;
            }

            return isPainted;
        }

        public override void PaintNearPoints(Vector3 point, float distance)
        {
            if (!_isDetectingNow)
                return;

            float radiusOfPoint = _pixelSize / 2;

            for (int i = 0; i < _pixels.Count; i++)
            {
                bool isNear = Vector3.Distance(_pixels[i].PointPosition, point) <= (distance + radiusOfPoint);

                if (isNear)
                    _pixels[i].SetPainted(true);
            }
        }   
        
        public override void EraseNearPoints(Vector3 point, float distance)
        {
            if (!_isDetectingNow)
                return;

            float radiusOfPoint = _pixelSize / 2;

            for (int i = 0; i < _pixels.Count; i++)
            {
                bool isNear = Vector3.Distance(_pixels[i].PointPosition, point) <= (distance + radiusOfPoint);

                if (isNear)
                    _pixels[i].SetPainted(false);
            }
        }

        public override void Hide()
        {
            _isDetectingNow = false;
            gameObject.SetActive(false);
        }

        public override void HighlightNearPoints(Vector3 point, float distance)
        {
            float radiusOfPoint = _pixelSize / 2;

            for (int i = 0; i < _pixels.Count; i++)
            {
                bool isNear = Vector3.Distance(_pixels[i].PointPosition, point) <= (distance + radiusOfPoint);
                _pixels[i].SetHighlighted(isNear);
            }
        }
        
        public override void ClearCanvas()
        {
            _isDetectingNow = true;

            if(_pixels == null)
                return;
            
            for (int i = 0; i < _pixels.Count; i++)
            {
                _pixels[i].Reset();
            }
        }

        private void Update()
        {
            if (_isDetectingNow)
                UpdatePointsView();
        }

        private void UpdatePointsView()
        {
            for (int i = 0; i < _pixels.Count; i++)
            {
                _pixels[i].UpdateView();
            }
        }

        private byte[,] CreateDataFromPixels()
        {
            byte[,] drawedData = new byte[_columnsAmount, _pixelsInRowAmount];

            for (int i = 0; i < _pixels.Count; i++)
            {
                if (!_pixels[i].IsPainted)
                    continue;

                int pointIndexX = (int)_pixels[i].IndexInArray.x;
                int pointIndexY = (int)_pixels[i].IndexInArray.y;

                drawedData[pointIndexX, pointIndexY] = 1;
            }

            return drawedData;
        }
        
        private void RemoveAllPixels()
        {
            if (pixelsRoot != null)
                Destroy(pixelsRoot);
        }
    }
}
