using BeyondGraph.Elements;
using BeyondGraph.Elements.Impls;
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
        /// 当前处于连线状态
        /// </summary>
        private bool connecting;
        /// <summary>
        /// 记录鼠标点击后的初始位置
        /// </summary>
        private Point clickLocation;

        public Canvas()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
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
            else if (this.connecting)
            {
                this.connectingMoving(e);
            }
            else
            {
                this.hoveringMoving(e);
            }
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
                if (node.Caught(currLocation))
                {
                    if (this.theSelected != node)
                    {
                        node.Selected = true;
                    }
                    currSelected = node;
                    break;
                }
                else
                {
                    foreach (AnchorElement anchor in node.Anchors)
                    {
                        if (anchor.Caught(currLocation))
                        {
                            if (this.theSelected != anchor)
                            {
                                anchor.Selected = true;
                            }
                            currSelected = anchor;
                            break;
                        }
                    }
                }
            }
            #endregion

            #region 连接Select检查
            for (int idx = this.connections.Count - 1; idx >= 0; idx--)
            {
                ConnectionElement conn = this.connections[idx];
                if (conn.Caught(currLocation))
                {
                    if (this.theSelected != conn)
                    {
                        conn.Selected = true;
                    }
                    currSelected = conn;
                    break;
                }
            }
            #endregion

            // 更新Selected元素
            if (this.theSelected != currSelected)
            {
                if (this.theSelected != null)
                {
                    this.theSelected.Selected = false;
                }
                this.theSelected = currSelected;
            }


            this.draging = true;
        }
        private void rightMouseDown(MouseEventArgs e)
        {
            this.contextMenu.Show(this, e.Location);
        }
        private void hoveringMoving(MouseEventArgs e)
        {
            Point currLocation = e.Location;

            Element currHovered = null;

            // 进行节点hovering
            #region 节点Hover检查
            for (int idx = this.nodes.Count - 1; idx >= 0; idx--)
            {
                NodeElement node = this.nodes[idx];
                if (node.Caught(currLocation))
                {
                    if (this.theHovered != node)
                    {
                        node.Hovered = true;
                    }
                    currHovered = node;
                    break;
                }
                else
                {
                    foreach (AnchorElement anchor in node.Anchors)
                    {
                        if (anchor.Caught(currLocation))
                        {
                            if (this.theHovered != anchor)
                            {
                                anchor.Hovered = true;
                            }
                            currHovered = anchor;
                            break;
                        }
                    }
                }
            }
            #endregion

            #region 连接Hover检查
            for (int idx = this.connections.Count - 1; idx >= 0; idx--)
            {
                ConnectionElement conn = this.connections[idx];
                if (conn.Caught(currLocation))
                {
                    if (conn != this.theHovered)
                    {
                        conn.Hovered = true;
                    }
                    currHovered = conn;
                    break;
                }
            }
            #endregion

            // 更新Hovered元素
            if (currHovered != this.theHovered)
            {
                if (this.theHovered != null)
                {
                    this.theHovered.Hovered = false;
                }
                this.theHovered = currHovered;
            }
        }
        private void dragingMoving(MouseEventArgs e)
        {
            Point curr = e.Location;
            Point vector = new Point(curr.X - clickLocation.X, curr.Y - clickLocation.Y);
            if (this.theSelected != null)
            {
                this.theSelected.Move(vector);
                if (this.theSelected is NodeElement)
                {
                    foreach (AnchorElement anchor in (this.theSelected as NodeElement).Anchors)
                    {
                        anchor.Move(vector);
                    }
                }
            }
            this.clickLocation = curr;
        }
        private void connectingMoving(MouseEventArgs e)
        {

        }
    }
}
