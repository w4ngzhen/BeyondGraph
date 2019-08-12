using BeyondGraph.Elements;
using BeyondGraph.Elements.Impls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeyondGraph.Factorys
{
    public static class ElementFactory
    {
        public static AnchorElement AnchorElement(int x, int y, NodeElement owner, Canvas canvas)
        {
            return new Anchor(x, y, owner, canvas);
        }
        public static AnchorElement AnchorElement(AnchorPosition position, NodeElement owner, Canvas canvas)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner对象不能为空");
            }
            int x = 0, y = 0;
            switch (position)
            {
                case AnchorPosition.Top:
                    x = owner.X + owner.Width / 2;
                    y = owner.Y;
                    break;
                case AnchorPosition.Right:
                    x = owner.X + owner.Width;
                    y = owner.Y + owner.Height / 2;
                    break;
                case AnchorPosition.Bottom:
                    x = owner.X + owner.Width / 2;
                    y = owner.Y + owner.Height;
                    break;
                case AnchorPosition.Left:
                    x = owner.X;
                    y = owner.Y + owner.Height / 2;
                    break;
            }
            return new Anchor(x, y, owner, canvas);
        }
        public static ConnectionElement ConnectionElement(AnchorElement from, AnchorElement to, Canvas canvas)
        {
            return new Line(from, to, canvas);
        }
    }
    public enum AnchorPosition
    {
        Top, Right, Bottom, Left
    }
}
