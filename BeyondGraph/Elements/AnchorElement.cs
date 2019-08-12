using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements
{
    public abstract class AnchorElement : Element
    {
        /// <summary>
        /// 锚点的承载方式为一个点
        /// </summary>
        protected Point positon;
        /// <summary>
        /// 锚点所属节点
        /// </summary>
        protected NodeElement owner;
        public Point Position
        {
            get { return this.positon; }
            set
            {
                this.positon = value;
                this.Invalidate();
            }
        }
        public NodeElement Owner
        {
            get { return this.owner; }
        }
        /// <summary>
        /// 锚点的定义区域
        /// </summary>
        public Rectangle DefinedArea
        {
            get
            {
                int width = 5;
                Rectangle rect = new Rectangle(this.positon, new Size(width, width));
                rect.Offset(-width / 2, -width / 2);
                return rect;
            }
        }
        /// <summary>
        /// 锚点的有区域
        /// </summary>
        public Rectangle EffectiveArea
        {
            get
            {
                int inflate = 5;
                Rectangle rect = this.DefinedArea;
                rect.Inflate(inflate, inflate);
                return rect;
            }

        }
        /// <summary>
        /// 是否为游离的锚点
        /// </summary>
        public bool IsDissociative
        {
            get { return this.owner == null; }
        }
        public AnchorElement(int x, int y, NodeElement owner,Canvas canvas)
            : base(canvas)
        {
            this.positon = new Point(x, y);
            this.owner = owner;
        }
        public override bool CaughtBy(Point p)
        {
            return this.EffectiveArea.Contains(p);
        }
        public override void Move(Point vector)
        {
            this.positon.X += vector.X;
            this.positon.Y += vector.Y;
            this.Invalidate();
        }
        public override void Paint(Graphics g)
        {
            if (hovered || selected)
            {
                g.FillRectangle(new SolidBrush(Color.Black), this.EffectiveArea);
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.Black), this.DefinedArea);
            }
        }
        public override void Invalidate()
        {
            Rectangle renderArea = this.EffectiveArea;
            renderArea.Inflate(2, 2);
            this.canvas.Invalidate(renderArea);
        }
    }
}
