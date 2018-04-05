using System.Drawing;

namespace VisualGraph
{
    public class LinkColor
    {
        public int FIndex { get; set; }
        public int SIndex { get; set; }
        public Ellipse F { get; set; }
        public Ellipse S { get; set; }
        public int Weight { get; set; }
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

        public LinkColor()
        {
            Color = Color.FromArgb(0, 0, 0);
        }
    }
}