using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements.Impls
{
    public class Line : ConnectionElement
    {
        public Line(AnchorElement from, AnchorElement to, Canvas canvas) 
            : base(from, to, canvas)
        {
        }
    }
}
