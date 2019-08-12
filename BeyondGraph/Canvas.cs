using BeyondGraph.Elements;
using BeyondGraph.Elements.Impls;
using BeyondGraph.Factorys;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BeyondGraph
{
    public class Canvas : UserControl
    {
        private List<NodeElement> nodes = new List<NodeElement>();
        private List<ConnectionElement> connections = new List<ConnectionElement>();
        private ToolStripDropDownMenu contextMenu;
        /// <summary>
        /// 记录当前hovered的元素
        /// </summary>
        private Element theHovered;
        /// <summary>
        /// 记录当前selected的元素
        /// </summary>
        private Element theSelected;
        /// <summary>
        /// 当前处于拖动状态
        /// </summary>
        private bool draging;
        /// <summary>
        /// 记录鼠标点击后的初始位置
        /// </summary>
        private Point clickLocation;
        /// <summary>
        /// 在画布上拖动锚点产生的临时连接元素
        /// </summary>
        private ConnectionElement tempConnection;

        public Canvas()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true); // 双缓冲
            this.buildMenu();
        }

        private void buildMenu()
        {
            ToolStripDropDownItem addItem = new ToolStripMenuItem();
            addItem.Name = "addMenu";
            addItem.Text = "添加";

            ToolStripMenuItem addSimpleRectItem = new ToolStripMenuItem();
            addSimpleRectItem.Name = "addSimpleRectItem";
            addSimpleRectItem.Text = "SimpleRectangle";
            addSimpleRectItem.Click += (sender, e) => { AddNodeElement(); };

            addItem.DropDownItems.Add(addSimpleRectItem);

            contextMenu = new ToolStripDropDownMenu();
            contextMenu.Items.Add(addItem);
        }
        public void AddNodeElement()
        {
            Random rand = new Random();
            Point location = new Point(rand.Next(200), rand.Next(200));
            NodeElement nodeEle = new SimpleRectangle(location, new Size(80, 60), this);
            this.nodes.Add(nodeEle);
            this.nodes.ForEach(ele => ele.Invalidate());
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (ConnectionElement conn in connections)
            {
                conn.Paint(e.Graphics);
            }
            this.tempConnection?.Paint(e.Graphics);
            foreach (NodeElement node in nodes)
            {
                node.Paint(e.Graphics);
                foreach (AnchorElement anchor in node.Anchors)
                {
                    anchor.Paint(e.Graphics);
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.draging)
            {
                this.dragingMoving(e);
            }
            else
            {
                this.hoveringMoving(e);
            }
            this.Invalidate(); // 未处理Connection.Invalidate，所以暂时全局Invalidate
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Point mouseLocation = e.Location;
            MouseButtons button = e.Button;
            if (button == MouseButtons.Left)
            {
                leftMouseDown(e);
            }
            else if (button == MouseButtons.Right)
            {
                rightMouseDown(e);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.draging)
            {
                this.draging = false;
                AnchorElement tempTo;
                if (this.theSelected is AnchorElement && (tempTo = this.theSelected as AnchorElement).IsDissociative)
                {
                    // 检查是否有能够连接上的锚点
                    foreach (NodeElement node in this.nodes)
                    {
                        // 节点自身上的锚点不能相互连接
                        if (node == this.tempConnection.From.Owner)
                        {
                            continue;
                        }
                        AnchorElement candidate = node.AttachWith(tempTo);
                        if (candidate != null)
                        {
                            // 连接上了，则用候选锚点替换当前连接的tempTo锚点
                            // 释放当前tempTo锚点图像
                            // 将tempConnction加如到画布上上的连接集合
                            this.tempConnection.To = candidate;
                            this.connections.Add(this.tempConnection);
                        }
                    }
                }
                // 无论如何都该释放掉临时连接
                this.tempConnection = null;
                this.Invalidate();
            }
        }
        private void leftMouseDown(MouseEventArgs e)
        {
            Point currLocation = e.Location;
            this.clickLocation = currLocation;
            Element currSelected = null;

            #region 节点Select检查
            for (int idx = this.nodes.Count - 1; idx >= 0; idx--)
            {
                NodeElement node = this.nodes[idx];
                if (node.CaughtBy(currLocation))
                {
                    currSelected = node;
                    break;
                }
                else
                {
                    AnchorElement anchor = node.AnchorCaughtBy(currLocation);
                    if (anchor != null)
                    {
                        currSelected = anchor;
                        break;
                    }
                }
            }
            #endregion

            #region 连接Select检查
            for (int idx = this.connections.Count - 1; idx >= 0; idx--)
            {
                ConnectionElement conn = this.connections[idx];
                if (conn.CaughtBy(currLocation))
                {
                    currSelected = conn;
                    break;
                }
            }
            #endregion

            #region 更新当前被选择的元素
            if (currSelected != this.theSelected)
            {
                if (currSelected != null)
                {
                    currSelected.Selected = true;
                }
                if (this.theSelected != null)
                {
                    this.theSelected.Selected = false;
                }
                this.theSelected = currSelected;
            }
            #endregion

            this.draging = true;
            // 若为锚点，则开始进行连接行为
            if (this.theSelected is AnchorElement)
            {
                AnchorElement from = this.theSelected as AnchorElement;
                AnchorElement tempTo = ElementFactory.AnchorElement(currLocation.X, currLocation.Y, null, this);
                ConnectionElement tempConn = ElementFactory.ConnectionElement(from, tempTo, this);
                this.tempConnection = tempConn;
                from.Selected = false;
                tempTo.Selected = true;
                this.theSelected = tempTo;
            }
        }
        private void rightMouseDown(MouseEventArgs e)
        {
            this.contextMenu.Show(this, e.Location);
        }
        private void hoveringMoving(MouseEventArgs e)
        {
            Point currLocation = e.Location;

            Element currHovered = null;

            #region 节点Hover检查
            for (int idx = this.nodes.Count - 1; idx >= 0; idx--)
            {
                NodeElement node = this.nodes[idx];
                if (node.CaughtBy(currLocation))
                {
                    currHovered = node;
                    break;
                }
                else
                {
                    AnchorElement anchor = node.AnchorCaughtBy(currLocation);
                    if (anchor != null)
                    {
                        currHovered = anchor;
                        break;
                    }
                }
            }
            #endregion

            #region 连接Hover检查
            for (int idx = this.connections.Count - 1; idx >= 0; idx--)
            {
                ConnectionElement conn = this.connections[idx];
                if (conn.CaughtBy(currLocation))
                {
                    currHovered = conn;
                    break;
                }
            }
            #endregion

            #region 更新Hovered元素
            if (currHovered != this.theHovered)
            {
                if (currHovered != null)
                {
                    currHovered.Hovered = true;
                }
                if (this.theHovered != null)
                {
                    this.theHovered.Hovered = false;
                }
                this.theHovered = currHovered;
            }
            #endregion
        }
        private void dragingMoving(MouseEventArgs e)
        {
            Point curr = e.Location;
            Point vector = new Point(curr.X - clickLocation.X, curr.Y - clickLocation.Y);
            if (this.theSelected is NodeElement)
            {
                this.theSelected.Move(vector);
            }
            else if (this.theSelected is AnchorElement)
            {
                // 连接元素上游离的锚点才能拖动
                AnchorElement anchor = this.theSelected as AnchorElement;
                if (anchor.IsDissociative)
                {
                    anchor.Move(vector);
                }
            }
            this.clickLocation = curr;
        }
    }
}
