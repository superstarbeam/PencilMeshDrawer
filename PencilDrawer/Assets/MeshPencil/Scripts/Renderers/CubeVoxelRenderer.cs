using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshPencil.Renderers.VoxelRenderer
{
    public class CubeVoxelRenderer : VoxelRenderer
    {
        public override event Action<Mesh> MeshCreated;
        public override event Action<string> MeshCreateFailed;

        private float _adjustScale;
        private Vector3 _voxelScale;

        private Mesh _mesh;
        private List<Vector3> _vertices;
        private List<int> _triangles;

        private float _scale = 1f;
        private float _depth = 1f;

        public override void Initialize(float scale,float depth)
        {
            _scale = scale;
            _depth = depth;

            _adjustScale = _scale * 0.5f;
            _voxelScale = new Vector3(_adjustScale, _depth, _adjustScale);
        }

        public override void CreateMeshFromData(byte[,] data)
        {
            VoxelData voxelData = new VoxelData(data);

            GenerateVoxelMesh(voxelData);

            Mesh generatedMesh = GetMesh();
            
            if (generatedMesh == null)
            {
                MeshCreateFailed?.Invoke("Generated mesh == 0");
            }
            else
            {
                MeshCreated?.Invoke(generatedMesh);
            }
        }

        private void GenerateVoxelMesh(VoxelData data)
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();

            for (int y = 0; y < data.Depth; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    if (data.GetCell(x, y) == 0)
                    {
                        continue;
                    }

                    var cubePosition = new Vector3((float)x * _scale, 0, (float)y * _scale);

                    MakeCube(_voxelScale, cubePosition, x, y, data);
                }
            }
        }

        private void MakeCube(Vector3 cubeScale, Vector3 cubePosition, int x, int y, VoxelData data)
        {
            for (int i = 0; i < 6; i++)
            {
                if (data.GetNeighbor(x, y, (Direction)i) == 0)
                {
                    MakeFace((Direction)i, cubeScale, cubePosition);
                }
            }
        }

        private void MakeFace(Direction direction, Vector3 faceScale, Vector3 facePosition)
        {
            _vertices.AddRange(CubeMeshData.FaceVertices(direction, faceScale, facePosition));

            int verticesCount = _vertices.Count;

            _triangles.Add(verticesCount - 4);
            _triangles.Add(verticesCount - 4 + 1);
            _triangles.Add(verticesCount - 4 + 2);
            _triangles.Add(verticesCount - 4);
            _triangles.Add(verticesCount - 4 + 2);
            _triangles.Add(verticesCount - 4 + 3);
        }

        private Mesh GetMesh()
        {   
            _mesh = new Mesh();

            Vector3[] rotatedVertices = RotatedVertices(_vertices).ToArray();

            _mesh.vertices = rotatedVertices;
            _mesh.triangles = _triangles.ToArray();

            //Recalculate UV
            Vector2[] uvs = new Vector2[rotatedVertices.Length];

            for (int i = 0; i < rotatedVertices.Length; i++)
            {
                uvs[i] = new Vector2(rotatedVertices[i].x, rotatedVertices[i].y);
            }

            _mesh.uv = uvs;
            _mesh.RecalculateNormals();
            _mesh.Optimize();

            return _mesh;
        }

        private List<Vector3> RotatedVertices(List<Vector3> vertices)
        {
            Vector3 center = new Vector3(0, 0, 0);
            Quaternion newRotation = new Quaternion();
            newRotation.eulerAngles = new Vector3(0, 90, 90);
            //TODO Set for x-z axis

            List<Vector3> rotatedVertices = new List<Vector3>();

            for (int i = 0; i < vertices.Count; i++)
            {
                rotatedVertices.Add(newRotation * (vertices[i] - center) + center);
            }

            return rotatedVertices;
        }
    }
}