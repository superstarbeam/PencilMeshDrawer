using UnityEngine;

namespace MeshPencil.Demo.Glide
{
    public class GlideLevelDrawController : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _playerBackCamRenderTexture;
        [SerializeField] private GameObject _sideCam;

        private Rigidbody _playerRigidBody;

        public void StartGlide()
        {
            SetPlayerDynamicState();
            SetScondaryCameraView();
        }

        private void Awake()
        {
            _playerRigidBody = _player.GetComponent<Rigidbody>();
        }

        private void SetScondaryCameraView()
        {
            _playerBackCamRenderTexture.SetActive(true);
            _sideCam.SetActive(true);
        }

        private void SetPlayerDynamicState()
        {
            _playerRigidBody.isKinematic = false;
            _playerRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
}
