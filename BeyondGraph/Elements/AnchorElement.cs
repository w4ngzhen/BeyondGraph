using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements
{
    public abstract class AnchorElement : Element
    {
        /// <summary>
        /// 锚点所属节点
        /// </summary>
        protected NodeElement owner;
        public NodeElement Owner
        { 
            get { return this.owner; }
        }
    }
}
