using System.Collections.Generic;
using UnityEngine;

namespace MeshPencil.Common.Pivot
{
    public class PivotsData
    {
        public int LeftestIndex;
        public int RightestIndex;
        public int UpestIndex;
        public int DownestIndex;
        
        public List<Vector2> LeftestTouchIndex;
        public List<Vector2> RightestTouchIndex;
        public List<Vector2> UpestTouchIndex;
        public List<Vector2> DownestTouchIndex;

        public Vector3 StartDrawPoint;
        public Vector3 FinishDrawPoint;
    }

    public enum PivotPosition
    {
        Default = 0,
        UpperLeft = 1,
        UpperRight = 2,
        LowerLeft = 3,
        LowerRight = 4,
        TouchingLeft = 5,
        TouchingRight = 6,
        TouchingUp = 7,
        TouchingDown = 8,
        StartDrawPoint = 9,
        FinishDrawPoint = 10,
    }
}
