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
        {
            this.canvas = canvas;
            this.body = new Rectangle(x, y, width, height);
            this.anchors[0] = new Anchor(this.X + this.Width / 2, this.Y, this, canvas);
            this.anchors[1] = new Anchor(this.X + this.Width, this.Y + this.Height / 2, this, canvas);
            this.anchors[2] = new Anchor(this.X + this.Width / 2, this.Y + this.Height, this, canvas);
            this.anchors[3] = new Anchor(this.X, this.Y + this.Height / 2, this, canvas);
        }
        public override void Paint(Graphics g)
        {
            // 首先绘制背景
            g.FillRectangle(new SolidBrush(this.backgroundColor), this.body);
            // 根据元素状态绘制边框
            if (this.hovered)
            {
                g.DrawRectangle(new Pen(Color.Red, 1), this.body);
            }
            else if (this.selected)
            {
                g.DrawRectangle(new Pen(Color.Red, 1), this.body);
            }
            else
            {
                g.DrawRectangle(new Pen(this.borderColor, 1), this.body);
            }
            this.Invalidate();
        }
    }
}
