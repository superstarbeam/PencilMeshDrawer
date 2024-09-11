using UnityEngine;

namespace MeshPencil.Demo
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotateAxis;
        [SerializeField] private float _rotateForce;

        private void FixedUpdate()
        {
            transform.Rotate(_rotateAxis * _rotateForce * Time.deltaTime);
        }
    }
}