using System;
using System.Drawing;

namespace VisualGraph
{
    public class Ellipse
    {
        public int X { get; set; }
        public int Y { get; set; }
        private Color _color { get; set; }
        private SolidBrush _brush { get; set; }

        public SolidBrush Brush
        {
            get
            {
                return _brush;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                _brush = new SolidBrush(value);
            }
        }

        public const int R = 30;

        public Ellipse()
        {
            Color = Color.FromArgb(0, 0, 0);
        }

        public bool IsIn(int x, int y)
        {
            return Math.Pow(x - X, 2) + Math.Pow(y - Y, 2) <= Math.Pow(R, 2);
        }
    }
}