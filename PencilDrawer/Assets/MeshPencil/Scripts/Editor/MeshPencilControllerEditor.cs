using MeshPencil.Common.Controllers;
using UnityEditor;
using UnityEngine;

namespace MeshPencil.Editor.Controllers
{
    [CustomEditor(typeof(MeshPencilController))]
    public class MeshPencilControllerEditor : UnityEditor.Editor
    {
        private MeshPencilController _controller;
        private SerializedObject _controllerSerialized;

        #region Serialized Properties

        //Main Controllers
        private SerializedProperty _inputListener;
        private SerializedProperty _pixelDrawer;
        private SerializedProperty _mouseCursorDisplayer;
        private SerializedProperty _borderDisplayer;
        private SerializedProperty _voxelRenderer;
        private SerializedProperty _drawHistoryRegister;

        //Pixel Canvas
        private SerializedProperty _pixelsInRowAmount;
        private SerializedProperty _columnsAmount;
        private SerializedProperty _canvasAreaPreview;

        //Single Voxel Settings
        private SerializedProperty _scale;
        private SerializedProperty _depth;

        //Workflow settings
        private SerializedProperty _isMultipleDrawMode;
        private SerializedProperty _renderCamera;

        //Result options
        private SerializedProperty _resultMeshPrefab;
        private SerializedProperty _adjustToCanvasSize;
        private SerializedProperty _pivotPosition;
        private SerializedProperty _parentTransform;
        
        private SerializedProperty _isCorrectRotation;
        private SerializedProperty _correctedRotation;
        private SerializedProperty _isCorrectScale;
        private SerializedProperty _correctedScale;
        
        private SerializedProperty OnFinish;

        //Collider
        private SerializedProperty _colliderType;
        private SerializedProperty _addRigidBody;
        private SerializedProperty _colliderPhysicMaterial;

        //Save/Load options
        private SerializedProperty _saveMesh;
        private SerializedProperty _subfolderToSave;
        private SerializedProperty _savedFileName;

        #endregion

        #region FoldOuts

        private bool _mainControllersFoldOut;
        private bool _pixelsCanvasFoldOut;
        private bool _singleVoxelFoldOut;
        private bool _workflowFoldOut;
        private bool _resultFoldOut;
        private bool _colliderFoldOut;
        private bool _saveloadFoldOut;


        #endregion

        #region GUI Styles
        
        private GUIStyle _hintStyle;

        #endregion

        protected virtual void OnEnable()
        {
            SerializeTargerObject();
            SerializeProperties();
            CreateStyles();
        }

        private void SerializeProperties()
        {
            _inputListener = _controllerSerialized.FindProperty("_inputListener");
            _pixelDrawer = _controllerSerialized.FindProperty("_pixelDrawer");
            _mouseCursorDisplayer = _controllerSerialized.FindProperty("_mouseCursorDisplayer");
            _borderDisplayer = _controllerSerialized.FindProperty("_borderDisplayer");
            _voxelRenderer = _controllerSerialized.FindProperty("_voxelRenderer");
            _drawHistoryRegister = _controllerSerialized.FindProperty("_drawHistoryRegister");

            _pixelsInRowAmount = _controllerSerialized.FindProperty("_pixelsInRowAmount");
            _columnsAmount = _controllerSerialized.FindProperty("_columnsAmount");
            _canvasAreaPreview = _controllerSerialized.FindProperty("_canvasAreaPreview");

            _scale = _controllerSerialized.FindProperty("_scale");
            _depth = _controllerSerialized.FindProperty("_depth");

            _isMultipleDrawMode = _controllerSerialized.FindProperty("_isMultipleDrawMode");
            _renderCamera = _controllerSerialized.FindProperty("_renderCamera");

            _resultMeshPrefab = _controllerSerialized.FindProperty("_resultMeshPrefab");
            _adjustToCanvasSize = _controllerSerialized.FindProperty("_adjustToCanvasSize");
            _pivotPosition = _controllerSerialized.FindProperty("_pivotPosition");
            _parentTransform = _controllerSerialized.FindProperty("_parentTransform");
            
            _isCorrectRotation = _controllerSerialized.FindProperty("_isCorrectRotation");
            _correctedRotation = _controllerSerialized.FindProperty("_correctedRotation");
            _isCorrectScale = _controllerSerialized.FindProperty("_isCorrectScale");
            _correctedScale = _controllerSerialized.FindProperty("_correctedScale");
            
            OnFinish = _controllerSerialized.FindProperty("OnFinish");

            _colliderType = _controllerSerialized.FindProperty("_colliderType");
            _addRigidBody = _controllerSerialized.FindProperty("_addRigidBody");
            _colliderPhysicMaterial = _controllerSerialized.FindProperty("_colliderPhysicMaterial");

            _saveMesh = _controllerSerialized.FindProperty("_saveMesh");
            _subfolderToSave = _controllerSerialized.FindProperty("_subfolderToSave");
            _savedFileName = _controllerSerialized.FindProperty("_savedFileName");
        }

        private void SerializeTargerObject()
        {
            _controller = (MeshPencilController)target;
            _controllerSerialized = new SerializedObject(target);
        }

        private void CreateStyles()
        {
            _hintStyle = new GUIStyle();
            _hintStyle.fontStyle = FontStyle.Italic;
        }

        public override void OnInspectorGUI()
        {
            _controllerSerialized.Update();

            EditorGUI.BeginChangeCheck();
            
            _mainControllersFoldOut =
                EditorGUILayout.Foldout(_mainControllersFoldOut,
                    GetColliderTabName(ControllerSettingsTab.MainControllers));
            
            if (_mainControllersFoldOut)
            {
                EditorGUILayout.LabelField("Display draw area , create mesh/collider and other controllers",
                    _hintStyle);

                EditorGUILayout.PropertyField(_inputListener);
                EditorGUILayout.PropertyField(_pixelDrawer);
                EditorGUILayout.PropertyField(_mouseCursorDisplayer);
                EditorGUILayout.PropertyField(_borderDisplayer);
                EditorGUILayout.PropertyField(_voxelRenderer);
                EditorGUILayout.PropertyField(_drawHistoryRegister);
            }

            _pixelsCanvasFoldOut =
                EditorGUILayout.Foldout(_pixelsCanvasFoldOut, GetColliderTabName(ControllerSettingsTab.PixelsCanvas));


            if (_pixelsCanvasFoldOut)
            {
                EditorGUILayout.LabelField("Pixel canvas area size", _hintStyle);

                EditorGUILayout.PropertyField(_pixelsInRowAmount);
                EditorGUILayout.PropertyField(_columnsAmount);
                EditorGUILayout.PropertyField(_canvasAreaPreview);
            }

            _singleVoxelFoldOut =
                EditorGUILayout.Foldout(_singleVoxelFoldOut, GetColliderTabName(ControllerSettingsTab.SingleVoxel));


            if (_singleVoxelFoldOut)
            {
                EditorGUILayout.LabelField("Single voxel parameters", _hintStyle);

                EditorGUILayout.PropertyField(_scale);
                EditorGUILayout.PropertyField(_depth);
            }

            _workflowFoldOut =
                EditorGUILayout.Foldout(_workflowFoldOut, GetColliderTabName(ControllerSettingsTab.Workflow));

            if (_workflowFoldOut)
            {
                EditorGUILayout.LabelField("Pixel canvas behaviour", _hintStyle);

                EditorGUILayout.PropertyField(_isMultipleDrawMode);
                EditorGUILayout.PropertyField(_renderCamera);
            }

            _resultFoldOut =
                EditorGUILayout.Foldout(_resultFoldOut, GetColliderTabName(ControllerSettingsTab.Result));


            if (_resultFoldOut)
            {
                EditorGUILayout.LabelField("Final created mesh settings", _hintStyle);

                EditorGUILayout.PropertyField(_resultMeshPrefab);
                EditorGUILayout.PropertyField(_adjustToCanvasSize);
                EditorGUILayout.PropertyField(_pivotPosition);
                EditorGUILayout.PropertyField(_parentTransform);

                EditorGUILayout.PropertyField(_isCorrectRotation);

                if (_isCorrectRotation.boolValue)
                {
                    EditorGUILayout.PropertyField(_correctedRotation);
                }

                EditorGUILayout.PropertyField(_isCorrectScale);

                if (_isCorrectScale.boolValue)
                {
                    EditorGUILayout.PropertyField(_correctedScale);
                }

                EditorGUILayout.PropertyField(OnFinish);
            }

            _colliderFoldOut =
                EditorGUILayout.Foldout(_colliderFoldOut, GetColliderTabName(ControllerSettingsTab.Collider));
            

            if (_colliderFoldOut)
            {
                EditorGUILayout.LabelField("Collider generation settings", _hintStyle);

                EditorGUILayout.PropertyField(_colliderType);

                if (_colliderType.enumValueIndex != 0)
                {
                    EditorGUILayout.PropertyField(_addRigidBody);
                    EditorGUILayout.PropertyField(_colliderPhysicMaterial);
                }
            }

            _saveloadFoldOut =
                EditorGUILayout.Foldout(_saveloadFoldOut, GetColliderTabName(ControllerSettingsTab.SaveLoad));

            if (_saveloadFoldOut)
            {
                EditorGUILayout.LabelField("Save/Load drawn data", _hintStyle);

                EditorGUILayout.PropertyField(_saveMesh);
                if (_saveMesh.boolValue)
                {
                    EditorGUILayout.PropertyField(_subfolderToSave);
                    EditorGUILayout.PropertyField(_savedFileName);
                }

                if (Application.isPlaying)
                {
                    if (GUILayout.Button("Load Last Created Mesh"))
                    {
                        _controller.LoadLastCreatedMeshData();
                    }
                }
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                _controllerSerialized.ApplyModifiedProperties();
            }
        }

        private string GetColliderTabName(ControllerSettingsTab tab)
        {
            string tabName = "Settings";

            switch (tab)
            {
                case ControllerSettingsTab.None:
                    break;
                case ControllerSettingsTab.MainControllers:
                    tabName = "Main Controllers";
                    break;
                case ControllerSettingsTab.PixelsCanvas:
                    tabName = "Pixels Canvas";
                    break;
                case ControllerSettingsTab.SingleVoxel:
                    tabName = "Single Voxel";
                    break;
                case ControllerSettingsTab.Workflow:
                    tabName = "WorkFlow";
                    break;
                case ControllerSettingsTab.Result:
                    tabName = "On Result";
                    break;
                case ControllerSettingsTab.Collider:
                    tabName = "Collider";
                    break;
                case ControllerSettingsTab.SaveLoad:
                    tabName = "Save Load Mesh";
                    break;
            }
            
            return tabName;
        }
    }

    public enum ControllerSettingsTab
    {
        None = 0,
        MainControllers = 1,
        PixelsCanvas = 2,
        SingleVoxel = 3,
        Workflow = 4,
        Result = 5,
        Collider = 6,
        SaveLoad = 7
    }
}
