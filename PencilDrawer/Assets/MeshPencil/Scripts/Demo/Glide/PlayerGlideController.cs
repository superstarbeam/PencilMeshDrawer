using UnityEngine;

namespace MeshPencil.Demo.Glide
{
    public class PlayerGlideController : MonoBehaviour
    {
        [SerializeField] private GameObject _winnerText;
        [SerializeField] private GameObject _loseText;

        private void OnTriggerExit(UnityEngine.Collider collidedObject)
        {
            if (collidedObject.gameObject.name == "Flag Finish")
            {
                ShowWinState();
            }

            if (collidedObject.gameObject.name == "Spike Block")
            {
                ShowLoseState();
            }
        }

        private void ShowLoseState()
        {
            Destroy(gameObject);
            _loseText.SetActive(true);
        }

        private void ShowWinState()
        {
            Destroy(gameObject);
            _winnerText.SetActive(true);
        }
    }
}
