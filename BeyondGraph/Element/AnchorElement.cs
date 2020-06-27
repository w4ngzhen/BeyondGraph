using System.Drawing;

namespace BeyondGraph.Element
{
    public abstract class AnchorElement : Element
    {
        /// <summary>
        /// 锚点的承载方式为一个点
        /// </summary>
        protected Point position;

        /// <summary>
        /// 锚点所属节点
        /// </summary>
        protected NodeElement owner;

        public Point Position
        {
            get => this.position;
            set
            {
                this.position = value;
                this.Invalidate();
            }
        }

        public NodeElement Owner => this.owner;

        /// <summary>
        /// 锚点的定义区域
        /// </summary>
        public Rectangle DefinedArea
        {
            get
            {
                const int width = 5;
                Rectangle rect = new Rectangle(this.position, new Size(width, width));
                rect.Offset(-width / 2, -width / 2);
                return rect;
            }
        }

        /// <summary>
        /// 锚点的有效区域
        /// </summary>
        public Rectangle EffectiveArea
        {
            get
            {
                const int inflate = 5;
                Rectangle rect = this.DefinedArea;
                rect.Inflate(inflate, inflate);
                return rect;
            }
        }

        /// <summary>
        /// 是否为游离的锚点
        /// </summary>
        public bool IsDissociative => this.owner == null;

        protected AnchorElement(int x, int y, NodeElement owner, Canvas canvas)
            : base(canvas)
        {
            this.position = new Point(x, y);
            this.owner = owner;
        }

        public override bool CaughtBy(Point p)
        {
            return this.EffectiveArea.Contains(p);
        }

        public override void Move(Point vector)
        {
            this.position.X += vector.X;
            this.position.Y += vector.Y;
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