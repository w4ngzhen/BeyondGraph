using System.Collections.Generic;
using System.Drawing;
using BeyondGraph.Util;

namespace BeyondGraph.Element
{
    public abstract class ConnectionElement : Element
    {
        /// <summary>
        /// 起始锚点
        /// </summary>
        protected AnchorElement from;

        /// <summary>
        /// 终止锚点
        /// </summary>
        protected AnchorElement to;

        /// <summary>
        /// 连接的样式
        /// </summary>
        protected ConnecitionSytle style;

        public AnchorElement From
        {
            get => this.from;
            set
            {
                this.from = value;
                this.Invalidate();
            }
        }

        public AnchorElement To
        {
            get => this.to;
            set
            {
                this.to = value;
                this.Invalidate();
            }
        }

        public ConnecitionSytle Style
        {
            get => this.style;
            set
            {
                this.style = value;
                this.Invalidate();
            }
        }

        protected ConnectionElement(AnchorElement from, AnchorElement to, Canvas canvas)
            : base(canvas)
        {
            this.from = from;
            this.to = to;
        }

        public override bool CaughtBy(Point p)
        {
            //return MathUtil.PointNearByLine(p, from.Position, to.Position);
            PointF left, right, tmp;
            left = from.Position;
            right = to.Position;
            if (left.X > right.X)
            {
                tmp = right;
                right = left;
                left = tmp;
            }

            float k = (right.Y - left.Y) / (right.X - left.X);
            int w = 5;
            PointF p1, p2, p3, p4;
            if (k - 1 > 0.0001 || k + 1 < 0.0001)
            {
                // 约为45度到135度之间
                // 水平平移
                p1 = new PointF(left.X - w, left.Y);
                p2 = new PointF(left.X + w, left.Y);
                p3 = new PointF(right.X - w, right.Y);
                p4 = new PointF(right.X + w, right.Y);
            }
            else
            {
                // 约为0-45度以及135度到180度
                // 上下平移
                p1 = new PointF(left.X, left.Y - w);
                p2 = new PointF(left.X, left.Y + w);
                p3 = new PointF(right.X, right.Y - w);
                p4 = new PointF(right.X, right.Y + w);
            }

            return MathUtil.IsInPolygon(p, new List<PointF>() {p1, p3, p4, p2});
        }

        public override void Move(Point vector)
        {
            // 连接本身不能移动，而是以来锚点移动
        }

        public override void Paint(Graphics g)
        {
            if (hovered || selected)
            {
                g.DrawLine(new Pen(Color.Red), from.Position, to.Position);
            }
            else
            {
                g.DrawLine(new Pen(Color.Black), from.Position, to.Position);
            }
        }

        public override void Invalidate()
        {
            Rectangle fromRect = new Rectangle(from.Position, new Size(1, 1));
            Rectangle toRect = new Rectangle(to.Position, new Size(1, 1));
            Rectangle union = Rectangle.Union(fromRect, toRect);
            this.canvas.Invalidate(union);
        }
    }

    public enum ConnecitionSytle
    {

    }
}