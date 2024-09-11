using MeshPencil.Common.Controllers;
using System;
using System.Collections;
using UnityEngine;

namespace MeshPencil.Demo.CharacterCreator
{
    public class CharacterCreatorController : MonoBehaviour
    {
        [SerializeField] private Animator _animatedCharacter;

        [SerializeField] private CharacterPartData[] _createdPartsData;

        [SerializeField] private MeshPencilController[] _meshPencilControllers;

        [SerializeField] private GameObject[] _steps;

        private int _currentStepIndex;

        private void Awake()
        {
            SubscribeEvents();

            ShowCurrentStep();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            for (int i = 0; i < _meshPencilControllers.Length; i++)
            {
                _meshPencilControllers[i].OnFinish.AddListener(MoveToNextStep);
            }
        }

        private void UnsubscribeEvents()
        {
            for (int i = 0; i < _meshPencilControllers.Length; i++)
            {
                _meshPencilControllers[i].OnFinish.RemoveListener(MoveToNextStep);
            }
        }

        private void MoveToNextStep()
        {
            _currentStepIndex++;

            if (_currentStepIndex == _steps.Length)
            {
                FinishDrawing();
            }
            else
            {
                ShowCurrentStep();
                HidePreviousStep();
            }
        }

        private void FinishDrawing()
        {
            AttachCratedObjectsToBones();
            HidePreviousStep();

            StartCoroutine(ShowFinalCharacterAfterDelay());
        }

        private IEnumerator ShowFinalCharacterAfterDelay()
        {
            yield return new WaitForSeconds(1);
            ShowFinalCharacter();
        }

        private void ShowFinalCharacter()
        {
            _animatedCharacter.enabled = true;
            _animatedCharacter.transform.position = Vector3.zero;
            _animatedCharacter.transform.localScale = new Vector3(0.5f, 0.5f, 1);

            Rotator rotator = _animatedCharacter.GetComponent<Rotator>();

            if (rotator!= null)
            {
                rotator.enabled = true;
            }
        }

        private void ShowCurrentStep()
        {
            if (_currentStepIndex >= _steps.Length)
                return;

            StartCoroutine(SmoothMoveToCameraSize());
            _steps[_currentStepIndex].gameObject.SetActive(true);
        }

        private IEnumerator SmoothMoveToCameraSize()
        {
            float cameraStartSize = Camera.main.orthographicSize;
            float cameraTargetSize = _createdPartsData[_currentStepIndex].CameraSize;

            float lerpTime = 0f;

            while (lerpTime < 1f)
            {
                Camera.main.orthographicSize = Mathf.Lerp(cameraStartSize, cameraTargetSize, lerpTime);
                lerpTime += 0.01f;

                yield return null;
            }
        }

        private void HidePreviousStep()
        {
            if (_currentStepIndex == 0)
                return;

            _steps[_currentStepIndex - 1].gameObject.SetActive(false);
        }

        private void AttachCratedObjectsToBones()
        {
            for (int i = 0; i < _createdPartsData.Length; i++)
            {
                for (int k = 0; k < _createdPartsData[i].TargetBoneRoot.Length; k++)
                {
                    Transform bodyPartTransform = Instantiate(_createdPartsData[i].CreatedPartPivotObject).transform;

                    bodyPartTransform.gameObject.SetActive(true);
                    bodyPartTransform.SetParent(_createdPartsData[i].TargetBoneRoot[k]);
                    bodyPartTransform.localPosition = Vector3.zero;
                }
            }
        }
    }
}


