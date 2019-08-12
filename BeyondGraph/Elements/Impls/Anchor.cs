﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeyondGraph.Elements.Impls
{
    public class Anchor : AnchorElement
    {
        public Anchor(int x, int y, NodeElement owner, Canvas canvas) : base(x, y, owner, canvas)
        {
        }
    }
}
