using UnityEngine;

namespace MeshPencil.Renderers.VoxelRenderer
{
    public static class CubeMeshData
    {
        private static Vector3[] Vertices =
        {
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(1, 1, -1),
            new Vector3(1, -1, -1),
            new Vector3(-1, -1, -1)
        };

        private static int[][] FaceTriangles =
        {
            new int[] {0, 1, 2, 3},
            new int[] {5, 0, 3, 6},
            new int[] {4, 5, 6, 7},
            new int[] {1, 4, 7, 2},
            new int[] {5, 4, 1, 0},
            new int[] {3, 2, 7, 6}
        };

        public static Vector3[] FaceVertices(Direction direction, Vector3 scale, Vector3 position)
        {
            Vector3[] faceVertices = new Vector3[4];

            for (int i = 0; i < faceVertices.Length; i++)
            {
                faceVertices[i] = Vector3.Scale(Vertices[FaceTriangles[(int) direction][i]], scale) + position;
            }

            return faceVertices;
        }
    }
}
