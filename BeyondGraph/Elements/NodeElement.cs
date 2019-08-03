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
        /// 元素默认上、右、下、左四个锚点
        /// </summary>
        protected AnchorElement[] anchors = new AnchorElement[4];
        public AnchorElement[] Anchors
        {
            get { return this.anchors; }
        }
        /// <summary>
        /// 传入鼠标，返回被鼠标捕获的锚点
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public AnchorElement AnchorCaught(Point p)
        {
            foreach (AnchorElement anchor in anchors)
            {
                if (anchor.Caught(p))
                {
                    return anchor;
                }
            }
            return null;
        }
    }
}
