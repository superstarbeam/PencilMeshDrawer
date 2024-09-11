namespace MeshPencil.Renderers.VoxelRenderer
{
    public class VoxelData
    {
        private byte[,] _data = new byte[,]
        {
            {0, 1, 0}
        };

        public VoxelData(byte[,] data)
        {
            SetData(data);
        }

        public void SetData(byte[,] data)
        {
            _data = data;
        }

        public int Width => _data.GetLength(0);

        public int Depth => _data.GetLength(1);

        public int GetCell(int x, int z)
        {
            return _data[x, z];
        }

        public int GetNeighbor(int x, int z, Direction dir)
        {
            DataCoordinate offsetToCheck = offsets[(int) dir];
            DataCoordinate neighborCoordinate =
                new DataCoordinate(x + offsetToCheck.x, 0 + offsetToCheck.y, z + offsetToCheck.z);

            if (neighborCoordinate.x < 0 ||
                neighborCoordinate.x >= Width ||
                neighborCoordinate.y != 0 ||
                neighborCoordinate.z < 0 ||
                neighborCoordinate.z >= Depth)
            {
                return 0;
            }
            else
            {
                return GetCell(neighborCoordinate.x, neighborCoordinate.z);
            }
        }

        private struct DataCoordinate
        {
            public int x;
            public int y;
            public int z;

            public DataCoordinate(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        private readonly DataCoordinate[] offsets =
        {
            new DataCoordinate(0, 0, 1),
            new DataCoordinate(1, 0, 0),
            new DataCoordinate(0, 0, -1),
            new DataCoordinate(-1, 0, 0),
            new DataCoordinate(0, 1, 0),
            new DataCoordinate(0, -1, 0),
        };
    }

    public enum Direction
    {
        North,
        Ease,
        South,
        West,
        Up,
        Down
    }
}

