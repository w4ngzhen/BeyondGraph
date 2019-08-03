using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements
{
    public abstract class Element
    {
        /// <summary>
        /// 外部画布
        /// </summary>
        protected Canvas canvas;
        /// <summary>
        /// 承载元素的矩形对象
        /// </summary>
        protected Rectangle body;
        /// <summary>
        /// 元素的x坐标
        /// </summary>
        protected int x;
        /// <summary>
        /// 元素的y坐标
        /// </summary>
        protected int y;
        /// <summary>
        /// 元素的宽度
        /// </summary>
        protected int width;
        /// <summary>
        /// 元素的高度
        /// </summary>
        protected int height;
        /// <summary>
        /// 是否被hover
        /// </summary>
        protected bool hovered;
        /// <summary>
        /// 是否被select
        /// </summary>
        protected bool selected;
        /// <summary>
        /// 元素边线颜色
        /// </summary>
        protected Color borderColor = Color.Black;
        /// <summary>
        /// 元素背景颜色
        /// </summary>
        protected Color backgroundColor = Color.White;

        public Canvas Canvas { get { return this.canvas; }}

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
        public bool Hovered
        {
            get { return this.hovered; }
            set
            {
                this.hovered = value;
                this.Invalidate();
            }
        }
        public bool Selected
        {
            get { return this.selected; }
            set
            {
                this.selected = value;
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
        /// <summary>
        /// 传入鼠标定位，返回鼠标是否捕获到该元素
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual bool Caught(Point p)
        {
            Rectangle theMouse = new Rectangle(p, new Size(1, 1));
            return this.body.Contains(theMouse);
        }
        /// <summary>
        /// 对元素进行移动
        /// </summary>
        /// <param name="vector">相对于某个起始点的移动向量，只是利用了Point数据结构</param>
        public virtual void Move(Point vector)
        {
            this.body.X += vector.X;
            this.body.Y += vector.Y;
            this.Invalidate();
        }
        /// <summary>
        /// 绘制，具体由子类实现
        /// </summary>
        /// <param name="g"></param>
        public abstract void Paint(Graphics g);
        /// <summary>
        /// 是指定画布区域失效并重新绘制，具体由子类实现
        /// </summary>
        public virtual void Invalidate()
        {
            Rectangle renderArea = this.body;
            renderArea.Inflate(5, 5);
            this.canvas.Invalidate(renderArea);
        }
    }
}
