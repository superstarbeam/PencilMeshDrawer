using System.Collections.Generic;
using MeshPencil.Common.Controllers;
using MeshPencil.Demo;
using UnityEngine;
using UnityEngine.UI;

public class TowerBuildController : MonoBehaviour
{
    [SerializeField] private MeshPencilController _meshPencilController;
    [SerializeField] private float _stepPositionOffset;
    [SerializeField] private int _totalStepsCount;
    [Space]
    [SerializeField] private GameObject _demoCamera;
    [SerializeField] private GameObject _createdTower;

    [SerializeField] private Text _currentStepText;

    private List<GameObject> _spawnedObjects;

    private int _currentStep;

    private void Start()
    {
        _spawnedObjects = new List<GameObject>();
        
        _meshPencilController.OnFinalObjectSpawned += AddObjectToSpawnedArray;
    }

    private void AddObjectToSpawnedArray(GameObject spawnedObject)
    {
        _spawnedObjects.Add(spawnedObject);
    }

    public void MoveToNextStep()
    {
        _currentStep++;

        if (_currentStep >= _totalStepsCount)
        {
            FinishDrawing();
            return;
        }

        MoveMeshPencilToNewPosition();
        UpdateCurrentStepText();
    }

    private void UpdateCurrentStepText()
    {
        _currentStepText.text = string.Format("You are drawing {0} platform from {1}", _currentStep, _totalStepsCount);
    }

    private void MoveMeshPencilToNewPosition()
    {
        Vector3 meshPencilPosition = _meshPencilController.gameObject.transform.position;

        Vector3 meshPencilOffsetedPosition = new Vector3(meshPencilPosition.x
            , meshPencilPosition.y,
            meshPencilPosition.z += _stepPositionOffset);

        _meshPencilController.gameObject.transform.position = meshPencilOffsetedPosition;
    }

    public void FinishDrawing()
    {
        foreach (var spawnedObject in _spawnedObjects)
        {
            spawnedObject.transform.parent = _createdTower.transform;
        }
        
        _meshPencilController.gameObject.SetActive(false);

        _demoCamera.SetActive(true);
        _createdTower.GetComponent<Rotator>().enabled = true;
    }
}
