using UnityEngine;

namespace MeshPencil.Drawers.PixelDrawer
{
    [CreateAssetMenu(fileName = "SpritePixelData", menuName = "MeshPencil/SpritePixelData", order = 1)]
    public class SpritePixelData : ScriptableObject
    {
        [SerializeField] private Color32 _undetectedColor;
        [SerializeField] private Color32 _detectedColor;
        [SerializeField] private Color32 _paintedColor;
        [SerializeField] private Color32 _erasedColor;

        public Color32 UndetectedColor
        {
            get
            {
                return _undetectedColor;
            }
        }

        public Color32 DetectedColor
        {
            get
            {
                return _detectedColor;
            }
        }

        public Color32 PaintedColor
        {
            get
            {
                return _paintedColor;
            }
        }

        public Color32 ErasedColor
        {
            get
            {
                return _erasedColor;
            }
        }
    }
}
