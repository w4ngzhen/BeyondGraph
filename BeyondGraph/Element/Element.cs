using System.Drawing;

namespace BeyondGraph.Element
{
    public abstract class Element
    {
        /// <summary>
        /// 外部画布
        /// </summary>
        protected Canvas canvas;
        /// <summary>
        /// 是否被hover
        /// </summary>
        protected bool hovered;
        /// <summary>
        /// 是否被select
        /// </summary>
        protected bool selected;

        public Canvas Canvas => this.canvas;

        public bool Hovered
        {
            get => this.hovered;
            set
            {
                this.hovered = value;
                this.Invalidate();
            }
        }
        public bool Selected
        {
            get => this.selected;
            set
            {
                this.selected = value;
                this.Invalidate();
            }
        }

        protected Element(Canvas canvas)
        {
            this.canvas = canvas;
        }

        /// <summary>
        /// 传入鼠标定位，返回鼠标是否捕获到该元素
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract bool CaughtBy(Point p);
        /// <summary>
        /// 对元素进行移动
        /// </summary>
        /// <param name="vector">相对于某个起始点的移动向量，只是利用了Point数据结构</param>
        public abstract void Move(Point vector);
        /// <summary>
        /// 绘制，具体由子类实现
        /// </summary>
        /// <param name="g"></param>
        public abstract void Paint(Graphics g);
        /// <summary>
        /// 是指定画布区域失效并重新绘制，具体由子类实现
        /// </summary>
        public abstract void Invalidate();
    }
}
