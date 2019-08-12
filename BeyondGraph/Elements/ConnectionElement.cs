using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements
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
            get { return this.from; }
            set
            {
                this.from = value;
                this.Invalidate();
            }
        }
        public AnchorElement To
        {
            get { return this.to; }
            set
            {
                this.to = value;
                this.Invalidate();
            }
        }
        public ConnecitionSytle Style
        {
            get { return this.style; }
            set
            {
                this.style = value;
                this.Invalidate();
            }
        }

        public ConnectionElement(AnchorElement from, AnchorElement to, Canvas canvas) 
            : base(canvas)
        {
            this.from = from;
            this.to = to;
        }

        public override bool CaughtBy(Point p)
        {
            return false;
        }

        public override void Move(Point vector)
        {
            // 连接本身不能移动，而是以来锚点移动
        }

        public override void Paint(Graphics g)
        {
            g.DrawLine(new Pen(Color.Black), from.Position, to.Position);
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
