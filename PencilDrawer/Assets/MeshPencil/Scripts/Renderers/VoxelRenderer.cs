using System;
using UnityEngine;

namespace MeshPencil.Renderers.VoxelRenderer
{
    public abstract class VoxelRenderer : MonoBehaviour
    {
        public abstract event Action<Mesh> MeshCreated;
        public abstract event Action<string> MeshCreateFailed;

        public abstract void CreateMeshFromData(byte[,] data);

        public abstract void Initialize(float scale, float _depth);
    }
}
