using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BeyondGraph.Element;
using BeyondGraph.Element.Impl;
using BeyondGraph.Factory;

namespace BeyondGraph
{
    public class Canvas : UserControl
    {
        private readonly List<NodeElement> _nodes = new List<NodeElement>();

        private readonly List<ConnectionElement> _connections = new List<ConnectionElement>();

        private ToolStripDropDownMenu _contextMenu;
        /// <summary>
        /// 记录当前hovered的元素
        /// </summary>
        private Element.Element _theHovered;
        /// <summary>
        /// 记录当前selected的元素
        /// </summary>
        private Element.Element _theSelected;
        /// <summary>
        /// 当前处于拖动状态
        /// </summary>
        private bool _dragging;
        /// <summary>
        /// 记录鼠标点击后的初始位置
        /// </summary>
        private Point _clickLocation;
        /// <summary>
        /// 在画布上拖动锚点产生的临时连接元素
        /// </summary>
        private ConnectionElement _tempConnection;

        public Canvas()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true); // 双缓冲
            this.BuildMenu();
        }

        private void BuildMenu()
        {
            ToolStripDropDownItem addItem = new ToolStripMenuItem();
            addItem.Name = "addMenu";
            addItem.Text = "添加";

            ToolStripMenuItem addSimpleRectItem = new ToolStripMenuItem
            {
                Name = "addSimpleRectItem", Text = "SimpleRectangle"
            };
            addSimpleRectItem.Click += (sender, e) => { AddNodeElement(); };

            addItem.DropDownItems.Add(addSimpleRectItem);

            _contextMenu = new ToolStripDropDownMenu();
            _contextMenu.Items.Add(addItem);
        }
        public void AddNodeElement()
        {
            Random rand = new Random();
            Point location = new Point(rand.Next(200), rand.Next(200));
            this.AddNodeElement(location);
        }

        public void AddNodeElement(Point point)
        {
            NodeElement nodeEle = new SimpleRectangle(point, new Size(80, 60), this);
            this._nodes.Add(nodeEle);
            this._nodes.ForEach(ele => ele.Invalidate());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            foreach (ConnectionElement conn in _connections)
            {
                conn.Paint(g);
            }
            this._tempConnection?.Paint(g);
            foreach (NodeElement node in _nodes)
            {
                node.Paint(g);
                foreach (AnchorElement anchor in node.Anchors)
                {
                    anchor.Paint(g);
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this._dragging)
            {
                this.DraggingMoving(e);
            }
            else
            {
                this.HoveringMoving(e);
            }
            this.Invalidate(); // 未处理Connection.Invalidate，所以暂时全局Invalidate
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            MouseButtons button = e.Button;
            switch (button)
            {
                case MouseButtons.Left:
                    LeftMouseDown(e);
                    break;
                case MouseButtons.Right:
                    RightMouseDown(e);
                    break;
                case MouseButtons.None:
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!this._dragging)
            {
                return;
            }
            this._dragging = false;
            AnchorElement tempTo;
            if (this._theSelected is AnchorElement element && (tempTo = element).IsDissociative)
            {
                // 检查是否有能够连接上的锚点
                foreach (NodeElement node in this._nodes)
                {
                    // 节点自身上的锚点不能相互连接
                    if (node == this._tempConnection.From.Owner)
                    {
                        continue;
                    }
                    AnchorElement candidate = node.AttachWith(tempTo);
                    if (candidate != null)
                    {
                        // 连接上了，则用候选锚点替换当前连接的tempTo锚点
                        // 释放当前tempTo锚点图像
                        // 将tempConnction加如到画布上上的连接集合
                        this._tempConnection.To = candidate;
                        this._connections.Add(this._tempConnection);
                    }
                }
            }
            // 无论如何都该释放掉临时连接
            this._tempConnection = null;
            this.Invalidate();
        }
        private void LeftMouseDown(MouseEventArgs e)
        {
            Point currentLocation = e.Location;
            this._clickLocation = currentLocation;
            Element.Element currentSelected = null;

            #region 节点Select检查
            for (int idx = this._nodes.Count - 1; idx >= 0; idx--)
            {
                NodeElement node = this._nodes[idx];
                if (node.CaughtBy(currentLocation))
                {
                    currentSelected = node;
                    break;
                }
                else
                {
                    AnchorElement anchor = node.AnchorCaughtBy(currentLocation);
                    if (anchor != null)
                    {
                        currentSelected = anchor;
                        break;
                    }
                }
            }
            #endregion

            #region 连接Select检查
            for (int idx = this._connections.Count - 1; idx >= 0; idx--)
            {
                ConnectionElement conn = this._connections[idx];
                if (conn.CaughtBy(currentLocation))
                {
                    currentSelected = conn;
                    break;
                }
            }
            #endregion

            #region 更新当前被选择的元素
            if (currentSelected != this._theSelected)
            {
                if (currentSelected != null)
                {
                    currentSelected.Selected = true;
                }
                if (this._theSelected != null)
                {
                    this._theSelected.Selected = false;
                }
                this._theSelected = currentSelected;
            }
            #endregion

            this._dragging = true;
            // 若为锚点，则开始进行连接行为
            if (!(this._theSelected is AnchorElement from))
            {
                return;
            }
            AnchorElement tempTo = ElementFactory.AnchorElement(currentLocation.X, currentLocation.Y, null, this);
            ConnectionElement tempConn = ElementFactory.ConnectionElement(@from, tempTo, this);
            this._tempConnection = tempConn;
            @from.Selected = false;
            tempTo.Selected = true;
            this._theSelected = tempTo;
        }
        private void RightMouseDown(MouseEventArgs e)
        {
            this._contextMenu.Show(this, e.Location);
        }
        private void HoveringMoving(MouseEventArgs e)
        {
            Point currentLocation = e.Location;

            Element.Element currentHovered = null;

            #region 节点Hover检查
            for (int idx = this._nodes.Count - 1; idx >= 0; idx--)
            {
                NodeElement node = this._nodes[idx];
                if (node.CaughtBy(currentLocation))
                {
                    currentHovered = node;
                    break;
                }
                else
                {
                    AnchorElement anchor = node.AnchorCaughtBy(currentLocation);
                    if (anchor != null)
                    {
                        currentHovered = anchor;
                        break;
                    }
                }
            }
            #endregion

            #region 连接Hover检查
            for (int idx = this._connections.Count - 1; idx >= 0; idx--)
            {
                ConnectionElement conn = this._connections[idx];
                if (conn.CaughtBy(currentLocation))
                {
                    currentHovered = conn;
                    break;
                }
            }
            #endregion

            #region 更新Hovered元素
            if (currentHovered != this._theHovered)
            {
                if (currentHovered != null)
                {
                    currentHovered.Hovered = true;
                }
                if (this._theHovered != null)
                {
                    this._theHovered.Hovered = false;
                }
                this._theHovered = currentHovered;
            }
            #endregion
        }
        private void DraggingMoving(MouseEventArgs e)
        {
            Point current = e.Location;
            Point vector = new Point(current.X - _clickLocation.X, current.Y - _clickLocation.Y);
            if (this._theSelected is NodeElement)
            {
                this._theSelected.Move(vector);
            }
            else if (this._theSelected is AnchorElement anchor)
            {
                // 连接元素上游离的锚点才能拖动
                if (anchor.IsDissociative)
                {
                    anchor.Move(vector);
                }
            }
            this._clickLocation = current;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "Canvas";
            this.ResumeLayout(false);

        }
    }
}
