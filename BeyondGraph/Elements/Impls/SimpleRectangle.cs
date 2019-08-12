using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements.Impls
{
    public class SimpleRectangle : NodeElement
    {
        public SimpleRectangle(Canvas canvas) : this(0, 0, 50, 40, canvas) { }

        public SimpleRectangle(Point location, Size size, Canvas canvas)
            : this(location.X, location.Y, size.Width, size.Height, canvas) { }

        public SimpleRectangle(int x, int y, int width, int height, Canvas canvas)
            : base(x, y, width, height, canvas) { }
    }
}
