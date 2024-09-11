using System;
using UnityEngine;

namespace MeshPencil.Common.MouseInputListener
{
    public class InputListener : MonoBehaviour
    {
        public event Action<bool> PaintStateChange;

        public event Action<bool> EraseStateChange;

        public event Action<Vector3> CursorPositionChange;

        public event Action<bool> ErasingModeChange;

        public event Action<float> Scaling;

        public event Action DrawFinish;
        public event Action DrawStarted;
        public event Action RemoveAllMeshes;

        [Tooltip("Is finish drawing on mouse button up")]
        [SerializeField] private bool _isMouseUpFinishDrawMode;
        [SerializeField] private KeyCode _finishDrawingKeyCode;
        [SerializeField] private KeyCode _removeAllMeshesKeyCode;
        [Space]
        [SerializeField] private KeyCode _changeCursorRadiusUp;
        [SerializeField] private KeyCode _changeCursorRadiusDown;

        [SerializeField] private float _cursorRadiusScalingPower;

        private Camera _renderCamera;
        private float _zPositionOffset;

        private bool _isInitialized;
        private Vector3 _previousCursorPosition;

        private bool _isErasingMode = false;

        public void Initialize(Camera renderCamera,float zPositionOffset)
        {    
            _renderCamera = renderCamera;
            
            _zPositionOffset = zPositionOffset;

             _isInitialized = true;
        }

        public void SetErasingMode(bool isEnabled)
        {
            _isErasingMode = isEnabled;
            ErasingModeChange?.Invoke(isEnabled);
            EraseStateChange?.Invoke(isEnabled);
        }

        public Vector3 GetMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;

            mousePos.z = _zPositionOffset > _renderCamera.nearClipPlane
                ? _zPositionOffset
                : _renderCamera.nearClipPlane + _zPositionOffset;

            Vector3 normalizedPosition = _renderCamera.ScreenToWorldPoint(mousePos);

            return normalizedPosition;
        }    

        public void FinishDrawing()
        {
            DrawFinish?.Invoke();
        }

        public virtual void OnDrawFinished()
        {
            FinishDrawing();
        }

        public virtual void OnRemoveAllMeshes()
        {
            RemoveAllMeshes?.Invoke();
        }

        private void Update()
        {   
            if(!_isInitialized)
                return;

            CheckDrawingInput();
            CheckScalingInput();
            CheckErasingInput();
            CheckDrawFinish();
            CheckRemoveAllMeshes();
            CheckCursorPositionChange();
        }

        private void CheckDrawFinish()
        {
            if (_isMouseUpFinishDrawMode)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    OnDrawFinished();
                }
            }
            else
            {
                if (Input.GetKeyDown(_finishDrawingKeyCode))
                {
                    OnDrawFinished();
                }
            }
        }   
        
        private void CheckRemoveAllMeshes()
        {
            if (Input.GetKeyDown(_removeAllMeshesKeyCode))
            {
                OnRemoveAllMeshes();
            }
        }

        private void CheckCursorPositionChange()
        {
            if (_previousCursorPosition == Vector3.zero)
            {
                _previousCursorPosition = GetMousePosition();
            }

            Vector3 currentCursorPosition = GetMousePosition();

            if (currentCursorPosition != _previousCursorPosition)
            {   
                OnCursorPositionChange(currentCursorPosition);
                _previousCursorPosition = currentCursorPosition;
            }
        }

        //Start and finish erase detection
        private void CheckErasingInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                SetErasingMode(true);
            }
            if (Input.GetMouseButtonUp(1))
            {
                SetErasingMode(false);
            }
        }

        //Start and finish painting detection
        private void CheckDrawingInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_isErasingMode)
                {
                    OnPaintStateChanged(true);
                }
                else
                {
                    OnEraseStateChanged(true);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnPaintStateChanged(false);

                if (_isErasingMode)
                    OnEraseStateChanged(false);
            }
        }

        //Scaling cursor influence radius
        private void CheckScalingInput()
        {
            CheckMouseScalingInput();
            CheckKeyScalingInput();
        }

        private void CheckMouseScalingInput()
        {
            if (Input.GetKeyDown(_changeCursorRadiusUp)) // forward
            {
                Scaling(_cursorRadiusScalingPower);
            }
            if (Input.GetKeyDown(_changeCursorRadiusDown)) // back
            {
                Scaling(-_cursorRadiusScalingPower);
            }
        }

        private void CheckKeyScalingInput()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            {
                Scaling(_cursorRadiusScalingPower);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
            {
                Scaling(-_cursorRadiusScalingPower);
            }
        }

        protected virtual void OnPaintStateChanged(bool isStarted)
        {
            PaintStateChange?.Invoke(isStarted);
        }

        protected virtual void OnEraseStateChanged(bool isStarted)
        {
            EraseStateChange?.Invoke(isStarted);
        }

        protected virtual void OnScaling(float scalingPower)
        {
            Scaling?.Invoke(scalingPower);
        }

        protected virtual void OnCursorPositionChange(Vector3 cursorPosition)
        {
            CursorPositionChange?.Invoke(cursorPosition);
        }
    }
}