using System.Collections;
using MeshPencil.Drawers.Data;
using UnityEngine;

namespace MeshPencil.Common
{
    public class DrawHistoryRegister : MonoBehaviour
    {
        [SerializeField] private bool _showDrawPointsGizmos;
        
        private Camera _renderCamera;
        private CanvasAreaData _canvasAreaData;

        public Vector3? StartDrawPoint { get; private set; }
        public Vector3? FinishDrawPoint { get; private set; }
        
        public Vector3? StartInCanvasRegionDrawPoint { get; private set; }
        public Vector3? FinishInCanvasRegionDrawPoint { get; private set; }
        
        private IEnumerator _cursorRealtimeCheckerCoroutine;

        private bool _isCheckingNow;
        public void Initialize(Camera renderCamera,CanvasAreaData canvasAreaData)
        {
            _renderCamera = renderCamera;
            _canvasAreaData = canvasAreaData;
        }

        public void UnInitialize()
        {
            if (_cursorRealtimeCheckerCoroutine != null)
            {
                StopCoroutine(_cursorRealtimeCheckerCoroutine); 
            }
        }
        
        /// <summary>
        /// Returns current cursor position clamped by canvas regions
        /// </summary>
        public Vector3 GetCurrentClampedCursorPoint()
        {
            var realCursorPosition = GetCurrentRealCursorPosition();
            var clampedPoint = ClampPointToCanvasRegions(realCursorPosition);

            return clampedPoint;
        }

        public Vector3 GetCurrentRealCursorPosition()
        {
            return _renderCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        
        public void StartListening()
        {
            _cursorRealtimeCheckerCoroutine = CheckIsCursorInRegion();

            StartDrawPoint = null;
            FinishDrawPoint = null;
            StartInCanvasRegionDrawPoint = null;
            FinishInCanvasRegionDrawPoint = null;
            
            _isCheckingNow = true;
            StartCoroutine(_cursorRealtimeCheckerCoroutine);
        }

        public void FinishListening()
        {
            _isCheckingNow = false;
            FinishDrawPoint = GetCurrentRealCursorPosition();
            StopCoroutine(_cursorRealtimeCheckerCoroutine);
        }
        
        private IEnumerator CheckIsCursorInRegion()
        {
            StartDrawPoint = GetCurrentRealCursorPosition();
            
            while (_isCheckingNow)
            {
                var currentCursorPosition = GetCurrentRealCursorPosition();
                
                if (IsCursorInsideCanvasArea())
                {
                    if (StartInCanvasRegionDrawPoint == null)
                    {
                        StartInCanvasRegionDrawPoint = currentCursorPosition;
                    }

                    FinishInCanvasRegionDrawPoint = currentCursorPosition;
                }
                
                yield return null;
            }

            yield return null;
        }

        private Vector3 ClampPointToCanvasRegions(Vector3 pointToClamp)
        {
            float clampedX = Mathf.Clamp(pointToClamp.x, _canvasAreaData.LeftestPoint, _canvasAreaData.RightestPoint);
            float clampedY = Mathf.Clamp(pointToClamp.y, _canvasAreaData.DownestPoint, _canvasAreaData.UperPoint);

            Vector3 resultPoint = new Vector3(clampedX,clampedY,pointToClamp.z);
            
            return resultPoint;
        }

        private bool IsCursorInsideCanvasArea()
        {
            var realCursorPosition = GetCurrentRealCursorPosition();

            bool isCursorInsideCanvas = !(realCursorPosition.x < _canvasAreaData.LeftestPoint ||
                                          realCursorPosition.x > _canvasAreaData.RightestPoint ||
                                          realCursorPosition.y < _canvasAreaData.DownestPoint ||
                                          realCursorPosition.y > _canvasAreaData.UperPoint);

            return isCursorInsideCanvas;
        }

        private void OnDrawGizmos()
        {
            if(!_showDrawPointsGizmos)
                return;
            
            if (StartDrawPoint.HasValue)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(StartDrawPoint.Value,0.3f);
            }
            
            if (StartInCanvasRegionDrawPoint.HasValue)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(StartInCanvasRegionDrawPoint.Value,0.3f);
            }
            
            if (FinishInCanvasRegionDrawPoint.HasValue)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(FinishInCanvasRegionDrawPoint.Value,0.3f);
            }
            
            if (FinishDrawPoint.HasValue)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(FinishDrawPoint.Value,0.3f);
            }
        }
    }
}