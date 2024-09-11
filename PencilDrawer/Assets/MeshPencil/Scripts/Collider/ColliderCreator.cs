using System;
using MeshPencil.Renderers.VoxelRenderer;
using UnityEngine;

namespace MeshPencil.Collider
{
    public abstract class ColliderCreator : MonoBehaviour
    {
        public abstract void CreateCollider(VoxelData data, float depth, float boxSize,
            PhysicMaterial physicMaterial = null, Action<GameObject> onCompleteCallback = null);
    }
}
