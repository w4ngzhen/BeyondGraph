using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements.Impls
{
    public class Anchor : AnchorElement
    {
        public Anchor(int x, int y, NodeElement owner, Canvas canvas) : this(x, y, 3, 3, owner, canvas)
        { }
        public Anchor(int x, int y, int width, int height, NodeElement owner, Canvas canvas)
        {
            this.body = new Rectangle(x, y, width, height);
            this.owner = owner;
            this.canvas = canvas;
        }
        public override void Paint(Graphics g)
        {
            g.FillEllipse(new SolidBrush(this.backgroundColor), this.body);
            g.DrawEllipse(new Pen(this.borderColor), this.body);
        }
    }
}
