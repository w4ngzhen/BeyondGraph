using BeyondGraph.Factorys;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements
{
    public abstract class NodeElement : Element
    {
        /// <summary>
        /// 承载元素的矩形对象
        /// </summary>
        protected Rectangle body;
        /// <summary>
        /// 元素默认上、右、下、左四个锚点
        /// </summary>
        protected AnchorElement[] anchors = new AnchorElement[4];
        /// <summary>
        /// 元素边线颜色
        /// </summary>
        protected Color borderColor = Color.Black;
        /// <summary>
        /// 元素背景颜色
        /// </summary>
        protected Color backgroundColor = Color.White;

        public Rectangle Body { get { return this.body; } }

        public virtual int X
        {
            get { return this.body.X; }
            set
            {
                this.body.X = value;
                this.Invalidate();
            }
        }

        public virtual int Y
        {
            get { return this.body.Y; }
            set
            {
                this.body.Y = value;
                this.Invalidate();
            }
        }

        public virtual int Width
        {
            get { return this.body.Width; }
            set
            {
                this.body.Width = value;
                this.Invalidate();
            }
        }

        public virtual int Height
        {
            get { return this.body.Height; }
            set
            {
                this.body.Height = value;
                this.Invalidate();
            }
        }
        public Color BorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }

        public Color BackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }

        public AnchorElement[] Anchors
        {
            get { return this.anchors; }
        }
        public NodeElement(int x, int y, Canvas canvas) 
            : this(x, y, 50, 40, canvas) { }
        public NodeElement(Point location, Size size, Canvas canvas)
            : this(location.X, location.Y, size.Width, size.Height, canvas) { }
        public NodeElement(int x, int y, int width, int height, Canvas canvas) 
            : base(canvas)
        {
            this.body = new Rectangle(x, y, width, height);
            this.anchors[0] = ElementFactory.AnchorElement(AnchorPosition.Top, this, canvas);
            this.anchors[1] = ElementFactory.AnchorElement(AnchorPosition.Right, this, canvas);
            this.anchors[2] = ElementFactory.AnchorElement(AnchorPosition.Bottom, this, canvas);
            this.anchors[3] = ElementFactory.AnchorElement(AnchorPosition.Left, this, canvas);
        }

        /// <summary>
        /// 传入鼠标，返回被鼠标捕获的锚点
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public AnchorElement AnchorCaughtBy(Point p)
        {
            foreach (AnchorElement anchor in anchors)
            {
                if (anchor.CaughtBy(p))
                {
                    return anchor;
                }
            }
            return null;
        }

        public AnchorElement AttachWith(AnchorElement outer)
        {
            foreach (AnchorElement anchor in this.anchors)
            {
                if (anchor.EffectiveArea.IntersectsWith(outer.EffectiveArea))
                {
                    return anchor;
                }
            }
            return null;
        }

        public override bool CaughtBy(Point p)
        {
            return this.body.Contains(p);
        }
        public override void Move(Point vector)
        {
            this.body.X += vector.X;
            this.body.Y += vector.Y;
            foreach (AnchorElement anchor in this.anchors)
            {
                anchor.Move(vector);
            }
            this.Invalidate();
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
        }
        public override void Invalidate()
        {
            Rectangle renderArea = this.body;
            renderArea.Inflate(5, 5);
            this.canvas.Invalidate(renderArea);
        }
    }
}
