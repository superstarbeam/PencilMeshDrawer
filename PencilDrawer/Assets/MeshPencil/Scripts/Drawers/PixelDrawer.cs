using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshPencil.Drawers.PixelDrawer
{
    public abstract class PixelDrawer : MonoBehaviour
    {
        public abstract event Action<byte[,]> DrawFinished;
        public abstract event Action<string> DrawFinishFailed;

        public abstract void SpawnPixelObjects();
        public abstract void FinishDrawing();

        /// <summary>
        /// Mark near points as painted
        /// </summary>
        /// <param name="point">Cursor position</param>
        /// <param name="distance">Cursor radius</param>
        public abstract void PaintNearPoints(Vector3 point, float distance);

        /// <summary>
        /// Return near points to unpainted state
        /// </summary>
        /// <param name="point">Cursor position</param>
        /// <param name="distance">Cursor radius</param>
        public abstract void EraseNearPoints(Vector3 point, float distance);
        /// <summary>
        /// Mark near points as "including to cursor radius"
        /// </summary>
        /// <param name="point">Cursor position</param>
        /// <param name="distance">Cursor radius</param>
        public abstract void HighlightNearPoints(Vector3 point, float distance);

        /// <summary>
        /// Hide draw canvas
        /// </summary>
        public abstract void Hide();

        public abstract void ClearCanvas();

        public abstract void Initialize(PixelDrawerData data);
    }
}
