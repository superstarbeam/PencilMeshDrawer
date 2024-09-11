using System;
using MeshPencil.Common.MouseInputListener;
using MeshPencil.Drawers.PixelDrawer;
using System.Collections.Generic;
using MeshPencil.Collider;
using MeshPencil.Common.MeshDataLoader;
using MeshPencil.Common.Pivot;
using MeshPencil.Drawers.Data;
using UnityEngine;
using MeshPencil.Renderers.VoxelRenderer;
using UnityEngine.Events;

namespace MeshPencil.Common.Controllers
{
    public class MeshPencilController : MonoBehaviour
    {
        public UnityEvent OnFinish;
        public UnityEvent BeforeMeshSpawned;
        public event Action<GameObject> OnFinalObjectSpawned;

        public Camera RenderCamera {
            get { return _renderCamera != null ? _renderCamera : Camera.main; }
        }

        #region Serialized Fields

        //Main Controllers
        [SerializeField] private InputListener _inputListener;
        [SerializeField] private PixelDrawer _pixelDrawer;
        [SerializeField] private MouseCursorDisplayer _mouseCursorDisplayer;
        [SerializeField] private VoxelRenderer _voxelRenderer;
        [SerializeField] private BorderDisplayer _borderDisplayer;
        [SerializeField] private DrawHistoryRegister _drawHistoryRegister;

        //Pixel Canvas
        [Tooltip("How many pixels contains single row")]
        [SerializeField] private int _pixelsInRowAmount;

        [Tooltip("How many rows in canvas")]
        [SerializeField] private int _columnsAmount;

        [Tooltip("Enable preview gizmos in editor")]
        [SerializeField] private bool _canvasAreaPreview;

        //Single Voxel Settings
        [SerializeField] private float _scale = 1f;
        [SerializeField] private float _depth = 1f;

        //Workflow settings
        [Tooltip("True : you can draw unlimited times\n" +
                 "False : disable canvas after draw finish")]
        [SerializeField] private bool _isMultipleDrawMode;
        [Tooltip("Leave empty for using Camera with 'MainCamera' tag")]
        [SerializeField] private Camera _renderCamera;

        //Result options
        [SerializeField] private GameObject _resultMeshPrefab;

        [Tooltip("Set as child to object, could be empty\n" +
                 "Duplicate spawned mesh for every parent  Transform")]
        [SerializeField] private Transform[] _parentTransform;

        [Tooltip("True : Move and scale created mesh to exectly place on canvas\n" +
                 "False : Leave mesh by default scale and position")]
        [SerializeField] private bool _adjustToCanvasSize;
        
        [SerializeField] private bool _isCorrectRotation;
        [SerializeField] private Vector3 _correctedRotation;
        [SerializeField] private bool _isCorrectScale;
        [SerializeField] private Vector3 _correctedScale;

        [Tooltip("Trim empty and create parent object")]
        [SerializeField] private PivotPosition _pivotPosition;

        //Collider
        [Tooltip(" StaticCollider = Mesh collider for kinematic rigidbody \n"+
                 " DynamicCollider = Generate dynamic mesh collider for non kinematic rigidbody support")]
        [SerializeField] private ColliderType _colliderType;

        [Tooltip("Only works with collider type : DynamicCollider")]
        [SerializeField] bool _addRigidBody;

        [SerializeField] private PhysicMaterial _colliderPhysicMaterial;

        //Save/Load options
        [Tooltip("Save generated mesh as save file")]
        [SerializeField] bool _saveMesh;
        [Tooltip("Subfolder in persistent data name")]
        [SerializeField] string _subfolderToSave = "MeshDatas";
        [Tooltip("Save data file name without extension")]
        [SerializeField] string _savedFileName = "MyMeshName";

        #endregion

        private void Initialize()
        {
            SubscribeEvents();

            _spawnedMeshes = new List<GameObject>();
            
            _mouseCursorDisplayer.Show();
            
            float zOffset = _pixelDrawer.transform.position.z - RenderCamera.gameObject.transform.position.z;
            _inputListener.Initialize(RenderCamera,zOffset);
            _mouseCursorDisplayer.Initialize();

            PixelDrawerData pixelDrawerData = new PixelDrawerData
            {
            PixelSize = _scale,
            ColumnsAmount = _columnsAmount,
            PixelsInRowAmount = _pixelsInRowAmount
            };
            
            _pixelDrawer.Initialize(pixelDrawerData);
            _voxelRenderer.Initialize(_scale, _depth);

            var canvasAreaData = new CanvasAreaData
            {
                LeftestPoint = transform.position.x,
                RightestPoint = transform.position.x + _pixelsInRowAmount * _scale,
                UperPoint = transform.position.y + _columnsAmount * _scale,
                DownestPoint = transform.position.y
            };

            _isDrawTurnFinished = true;
            
            _drawHistoryRegister.Initialize(RenderCamera,canvasAreaData);
        }

        private void Uninitialize()
        {
            UnsubscribeEvents();
            _mouseCursorDisplayer.Hide();
            _drawHistoryRegister.UnInitialize();
        }

        #region Private Fields

        private byte[,] _lastDrawData;

        //Mouse pressed and marking point as painted , false when mouse|touch up 
        private bool _isPaintingNow;
        //Is current draw session finished, true when create mesh invoked, false when we still drawing
        private bool _isDrawTurnFinished;
        private bool _isErasingNow;

        private MeshFilter _resultMeshFilter;
        private List<GameObject> _spawnedMeshes;

        #endregion

        #region Getters
    
        /// <summary>
        /// Canvas pixels resolution
        /// </summary>
        /// <returns>Vector2 where
        /// X = count of pixels in the row
        /// Y = Count of columns </returns>
        public Vector2 GetCanvasResolution()
        {
            return new Vector2(_pixelsInRowAmount,_columnsAmount);
        }
        
        /// <summary>
        /// Single pixel scale
        /// </summary>
        /// <returns></returns>
        public float GetPixelScale()
        {
            return _scale;
        }
        
        /// <summary>
        /// Depth (Z scale) of a final mesh
        /// </summary>
        /// <returns></returns>
        public float GetPixelDepth()
        {
            return _depth;
        }
        
        /// <summary>
        /// Will canvas hide after drawing finish
        /// </summary>
        /// <returns></returns>
        public bool IsMultidrawMode()
        {
            return _isMultipleDrawMode;
        }
        
        /// <summary>
        /// Camera which renders MeshPencil
        /// </summary>
        /// <returns></returns>
        public Camera GetRenderCamera()
        {
            return RenderCamera;
        }
        
        /// <summary>
        /// Pivot position
        /// </summary>
        /// <returns></returns>
        public PivotPosition GetPivotPosition()
        {
            return _pivotPosition;
        }
        
        /// <summary>
        /// Collider interaction type
        /// </summary>
        /// <returns></returns>
        public ColliderType GetColliderType()
        {
            return _colliderType;
        }
        
        #endregion

        #region Setters

        /// <summary>
        /// Set canvas resolution and update canvas view immediately 
        /// </summary>
        /// <param name="resolution">Count of raws(X) and columns (y)</param>
        /// <param name="pixelScale">Single pixel size</param>
        public void SetCanvasResolution(Vector2 resolution,float pixelScale)
        {
            _pixelsInRowAmount = (int) resolution.x;
            _columnsAmount = (int) resolution.y;
            _scale = pixelScale;
            
            PixelDrawerData data = new PixelDrawerData
            {
            PixelsInRowAmount = (int)resolution.x,
            ColumnsAmount = (int)resolution.y,
            PixelSize = pixelScale
            };
            
            _pixelDrawer.Initialize(data);
            _voxelRenderer.Initialize(_scale, _depth);
        }

        /// <summary>
        /// Depth (Z scale) of a final mesh
        /// </summary>
        /// <param name="depth"></param>
        public void SetDepth(float depth)
        {
            _depth = depth;
            _voxelRenderer.Initialize(_scale, _depth);
        }
        
        /// <summary>
        /// Will canvas hide after drawing finish
        /// </summary>
        /// <param name="isEnabled">true : don't when drawing is finished</param>
        public void SetMultipleDrawMode(bool isEnabled)
        {
            _isMultipleDrawMode = isEnabled;
        }
        
        /// <summary>
        /// Set the GameObject result prefab.
        /// Require MeshFilter component on the prefab object
        /// </summary>
        /// <param name="resultMeshPrefab">prefab which will be spawned</param>
        public void SetResultMeshPrefab(GameObject resultMeshPrefab)
        {
            _resultMeshPrefab = resultMeshPrefab;
        }
        
        /// <summary>
        /// Transform which will be seted as a parent
        /// </summary>
        /// <param name="parentTransforms"></param>
        public void SetResultParentTransform(Transform[] parentTransforms)
        {
            _parentTransform = parentTransforms;
        }
        
        /// <summary>
        /// Rotation of a result object
        /// </summary>
        /// <param name="rotation"></param>
        public void SetResultRotation(Vector3 rotation)
        {
            _isCorrectRotation = true;
            _correctedRotation = rotation;
        }
        
        public void DisableRotationCorrection()
        {
            _isCorrectRotation = false;
        }
        
        /// <summary>
        /// Result object transform scale
        /// </summary>
        /// <param name="scale"></param>
        public void SetResultScale(Vector3 scale)
        {
            _isCorrectScale = true;
            _correctedScale = scale;
        }
        
        public void DisableScaleCorrection()
        {
            _isCorrectScale = false;
        }
        
        /// <summary>
        /// Where to place a pivot of a spawned mesh
        /// </summary>
        /// <param name="pivotPosition"></param>
        public void SetPivotPosition(PivotPosition pivotPosition)
        {
            _pivotPosition = pivotPosition;
        }
        
        /// <summary>
        /// Add Rigidbody component to a final mesh.
        /// Only works with Dynamic collider
        /// </summary>
        /// <param name="isAddRigidbody"></param>
        public void IsAddRigidbody(bool isAddRigidbody)
        {    
            if(_colliderType == ColliderType.None || _colliderType == ColliderType.StaticCollider)
                return;
            
            _addRigidBody = isAddRigidbody;
        }
        
        /// <summary>
        /// Physics material of a collider
        /// Requires ColliderType Static or Dynamic
        /// </summary>
        /// <param name="physicMaterial"></param>
        public void SetPhysicsMaterial(PhysicMaterial physicMaterial)
        {
            _colliderPhysicMaterial = physicMaterial;
        }
        
        /// <summary>
        /// Set the collider type of a final mesh
        /// </summary>
        /// <param name="colliderType"></param>
        public void SetColliderType(ColliderType colliderType)
        {
            _colliderType = colliderType;
        }
        
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Initialize();
        }

        private void OnDisable()
        {
            Uninitialize();
        }

        private void Update()
        {
            ListenPaintPixels();
            ListenErasePixels();

            HighlightPixels();
        }

        #endregion

        #region Save Load Data Methods

        public void SaveMeshData(string savedMeshFileName)
        {
            if (_lastDrawData == null)
            {
                Debug.LogError("Last draw data is null , please draw mesh before saving");
                return;
            }

            MeshDataSaveController.SaveData(_lastDrawData, _subfolderToSave, savedMeshFileName);
        }

        public void LoadMeshData(string savedMeshFileName)
        {
            byte[,] loadedData = MeshDataSaveController.LoadData(_subfolderToSave, savedMeshFileName);

            _lastDrawData = loadedData;

            CreateMeshFromLoadedData(loadedData);
        }

        public void LoadLastCreatedMeshData()
        {
            byte[,] loadedData = MeshDataSaveController.LoadData(_subfolderToSave, _savedFileName);

            _lastDrawData = loadedData;

            if (loadedData == null)
            {
                Debug.LogError("MeshPencilController : can't create mesh. loaded data is null");
                return;
            }
            CreateMeshFromLoadedData(loadedData);
        }

        private void CreateMeshFromLoadedData(byte[,] loadedData)
        {
            _voxelRenderer.CreateMeshFromData(loadedData);
        }

        #endregion

        #region Result Methods

        private void AdjustToCanvasSize()
        {
            Vector3 pixelDrawerPos = _pixelDrawer.gameObject.transform.position;

            Vector3 adjustedPosition = new Vector3(
                pixelDrawerPos.x + (_scale / 2),
                pixelDrawerPos.y + (_scale / 2),
                pixelDrawerPos.z);

            _resultMeshFilter.transform.position = adjustedPosition;
            _resultMeshFilter.transform.localRotation = _pixelDrawer.gameObject.transform.rotation;
            _resultMeshFilter.transform.localScale = _pixelDrawer.gameObject.transform.localScale;
        }

        #endregion

        #region Collider Methods

        private void CreateCollider(ColliderType colliderType)
        {
            switch (colliderType)
            {
                case ColliderType.StaticCollider:
                    AddStaticMeshCollider();
                    break;
                case ColliderType.DynamicCollider:
                    GenerateDynamicMeshCollider();
                    break;
            }
        }

        private void GenerateDynamicMeshCollider()
        {
            VoxelData voxelData = new VoxelData(_lastDrawData);

            var colliderCreator = _resultMeshFilter.GetComponent<ColliderCreator>();
            colliderCreator.CreateCollider(voxelData, _depth * 2, _scale,_colliderPhysicMaterial,ColliderCreator_OnColliderCreated);
        }

        private void AddStaticMeshCollider()
        {
            MeshCollider AddedMeshCollider = _resultMeshFilter.gameObject.AddComponent<MeshCollider>();
            if (_colliderPhysicMaterial != null)
            {
                AddedMeshCollider.material = _colliderPhysicMaterial;
            }
        }

        private void ColliderCreator_OnColliderCreated(GameObject colliderObject)
        {
            colliderObject.transform.parent = _resultMeshFilter.transform;

            if (_addRigidBody)
            {
                _resultMeshFilter.gameObject.AddComponent<Rigidbody>();
            }
        }

        #endregion

        #region Subscribed Listeners

        private void SubscribeEvents()
        {
            _inputListener.PaintStateChange += InputListener_OnPaintStateChange;
            _inputListener.EraseStateChange += InputListener_OnEraseStateChange;
            _inputListener.DrawFinish += InputListener_OnDrawFinish;
            _inputListener.RemoveAllMeshes += InputListener_OnRemoveAllMeshes;

            _pixelDrawer.DrawFinished += PixelDrawer_OnDrawFinished;
            _pixelDrawer.DrawFinishFailed += PixelDrawer_OnDrawFinishFailed;
            _voxelRenderer.MeshCreated += VoxelRenderer_OnMeshCreated;
        }

        private void UnsubscribeEvents()
        {
            _inputListener.PaintStateChange -= InputListener_OnPaintStateChange;
            _inputListener.EraseStateChange -= InputListener_OnEraseStateChange;
            _inputListener.DrawFinish -= InputListener_OnDrawFinish;
            _inputListener.RemoveAllMeshes -= InputListener_OnRemoveAllMeshes;

            _pixelDrawer.DrawFinished -= PixelDrawer_OnDrawFinished;
            _pixelDrawer.DrawFinishFailed -= PixelDrawer_OnDrawFinishFailed;
            _voxelRenderer.MeshCreated -= VoxelRenderer_OnMeshCreated;
        }

        private void InputListener_OnPaintStateChange(bool isPaintingNow)
        {
            if (isPaintingNow && _isDrawTurnFinished)
            {
                OnDrawStarted();
            }

            _isPaintingNow = isPaintingNow;
        }

        private void InputListener_OnEraseStateChange(bool isErasingNow)
        {
            _isErasingNow = isErasingNow;
        }

        private void HighlightPixels()
        {
            Vector3 positionToDraw = _inputListener.GetMousePosition();
            float drawPointRadius = _mouseCursorDisplayer.DrawCircleRadius;

            _pixelDrawer.HighlightNearPoints(positionToDraw, drawPointRadius);
        }

        private void ListenPaintPixels()
        {
            if (!_isPaintingNow)
                return;

            Vector3 positionToDraw = _inputListener.GetMousePosition();
            float drawPointRadius = _mouseCursorDisplayer.DrawCircleRadius;

            _pixelDrawer.PaintNearPoints(positionToDraw, drawPointRadius);
        }

        private void ListenErasePixels()
        {
            if (!_isErasingNow)
                return;

            Vector3 positionToDraw = _inputListener.GetMousePosition();
            float drawPointRadius = _mouseCursorDisplayer.DrawCircleRadius;

            _pixelDrawer.EraseNearPoints(positionToDraw, drawPointRadius);
        }

        private void InputListener_OnRemoveAllMeshes()
        {
            for (var i = 0; i < _spawnedMeshes.Count; i++)
            {
                Destroy(_spawnedMeshes[i]);
            }

            _spawnedMeshes = new List<GameObject>();
        }

        private void PixelDrawer_OnDrawFinished(byte[,] data)
        {
            _lastDrawData = data;
            
            _voxelRenderer.CreateMeshFromData(data);

            _pixelDrawer.ClearCanvas();

            if (!_isMultipleDrawMode)
            {
                _pixelDrawer.Hide();
                Uninitialize();
            }

            if (_saveMesh)
            {
                SaveMeshData(_savedFileName);
            }
        }

        private void PixelDrawer_OnDrawFinishFailed(string args)
        {
            _pixelDrawer.ClearCanvas();
        }

        private void OnDrawStarted()
        {
            _isDrawTurnFinished = false;
            if (_pivotPosition == PivotPosition.StartDrawPoint ||
                _pivotPosition == PivotPosition.FinishDrawPoint)
            {
                _drawHistoryRegister.StartListening();
            }
        }
        
        private void InputListener_OnDrawFinish()
        {
            _isDrawTurnFinished = true;
            if (_pivotPosition == PivotPosition.StartDrawPoint ||
                _pivotPosition == PivotPosition.FinishDrawPoint)
            {
                _drawHistoryRegister.FinishListening();
            }
            
            _pixelDrawer.FinishDrawing();
        }

        private void VoxelRenderer_OnMeshCreated(Mesh createdMesh)
        {
            BeforeMeshSpawned?.Invoke();
            
            _resultMeshFilter = Instantiate(_resultMeshPrefab).GetComponent<MeshFilter>();

            _spawnedMeshes.Add(_resultMeshFilter.gameObject);

            _resultMeshFilter.mesh = createdMesh;
            
            if (_colliderType != ColliderType.None)
            {
                CreateCollider(_colliderType);
            }
            
            if (_adjustToCanvasSize)
            {
                AdjustToCanvasSize();
            }

            Transform resultParentObject = _resultMeshFilter.gameObject.transform;
            
            if (_pivotPosition != PivotPosition.Default)
            {
                var borderDisplayerData = CreateBorderDisplayDataData();
                
                resultParentObject = _borderDisplayer.SetPivot(borderDisplayerData);
            }

            if (_parentTransform.Length > 0)
            {
                SetParentTransform(resultParentObject,_parentTransform[0]);
            }

            CorrectTransform(resultParentObject);

            //Dublicate object to multi parents array
            if (_parentTransform.Length > 1)
            {
                for (int i = 1; i < _parentTransform.Length; i++)
                {
                    Transform finalObjectCopy = Instantiate(resultParentObject);
                    SetParentTransform(finalObjectCopy,_parentTransform[i]);
                    CorrectTransform(finalObjectCopy);
                }
            }

            OnFinish?.Invoke();
            
            OnFinalObjectSpawned?.Invoke(resultParentObject.gameObject);
        }

        private BorderDisplayerData CreateBorderDisplayDataData()
        {
            var definedBorders = PivotDefiner.DefineBorderPoints(_lastDrawData);

            if (_drawHistoryRegister.StartInCanvasRegionDrawPoint.HasValue)
            {
                definedBorders.StartDrawPoint = _drawHistoryRegister.StartInCanvasRegionDrawPoint.Value;
            }
                
            if (_drawHistoryRegister.FinishInCanvasRegionDrawPoint.HasValue)
            {
                definedBorders.FinishDrawPoint = _drawHistoryRegister.FinishInCanvasRegionDrawPoint.Value;
            }
                
            BorderDisplayerData borderDisplayerData = new BorderDisplayerData
            {
                PivotPosition = _pivotPosition,
                PivotsData = definedBorders,
                PixelScale = _scale,
                SpawnedMesh = _resultMeshFilter.gameObject.transform
            };

            return borderDisplayerData;
        }

        private void CorrectTransform(Transform resultParentObject)
        {
            if (_isCorrectRotation)
            {    
                resultParentObject.localRotation = Quaternion.Euler(_correctedRotation);
            }

            if (_isCorrectScale)
            {
                resultParentObject.localScale = _correctedScale;
            }
        }
        
        private void SetParentTransform(Transform resultParentObject,Transform parentTransform)
        {
            resultParentObject.parent = parentTransform;
            resultParentObject.localPosition = Vector3.zero;
            resultParentObject.localRotation = Quaternion.Euler(0,0,0);
            resultParentObject.localScale = Vector3.one;
        }

        #endregion

        #region BorderDefine methods
        
        
        #endregion
        
        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!_canvasAreaPreview)
                return;

            Color gizmosColor = Color.white;
            gizmosColor.a /= 2;

            Gizmos.color = gizmosColor;

            Vector3 canvasSize = new Vector3(
                _pixelsInRowAmount * _scale,
                _columnsAmount * _scale,
                0f);

            Gizmos.DrawCube(transform.position + (canvasSize / 2), canvasSize);
        }

        #endregion
    }
}
