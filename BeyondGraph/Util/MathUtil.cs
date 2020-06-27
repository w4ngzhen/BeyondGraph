using System.Collections.Generic;
using System.Drawing;

namespace BeyondGraph.Util
{
    public static class MathUtil
    {
        public static bool IsInPolygon(PointF checkPoint, List<PointF> polygonPoints)
        {
            bool inside = false;
            int pointCount = polygonPoints.Count;
            PointF p1, p2;
            for (int i = 0, j = pointCount - 1;
                i < pointCount;
                j = i, i++) //第一个点和最后一个点作为第一条线，之后是第一个点和第二个点作为第二条线，之后是第二个点与第三个点，第三个点与第四个点...
            {
                p1 = polygonPoints[i];
                p2 = polygonPoints[j];
                if (checkPoint.Y < p2.Y)
                {
                    //p2在射线之上
                    if (p1.Y <= checkPoint.Y)
                    {
                        //p1正好在射线中或者射线下方
                        if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) > (checkPoint.X - p1.X) * (p2.Y - p1.Y)
                        ) //斜率判断,在P1和P2之间且在P1P2右侧
                        {
                            //射线与多边形交点为奇数时则在多边形之内，若为偶数个交点时则在多边形之外。
                            //由于inside初始值为false，即交点数为零。所以当有第一个交点时，则必为奇数，则在内部，此时为inside=(!inside)
                            //所以当有第二个交点时，则必为偶数，则在外部，此时为inside=(!inside)
                            inside = (!inside);
                        }
                    }
                }
                else if (checkPoint.Y < p1.Y)
                {
                    //p2正好在射线中或者在射线下方，p1在射线上
                    if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) < (checkPoint.X - p1.X) * (p2.Y - p1.Y)
                    ) //斜率判断,在P1和P2之间且在P1P2右侧
                    {
                        inside = (!inside);
                    }
                }
            }

            return inside;
        }

        public static bool PointNearByLine(PointF checkPoint, PointF lineP1, PointF lineP2)
        {
            PointF p1, p2, s;
            RectangleF r1, r2;
            float o, u;
            p1 = lineP1;
            p2 = lineP2;

            // p1 must be the leftmost point.
            if (p1.X > p2.X)
            {
                s = p2;
                p2 = p1;
                p1 = s;
            }

            r1 = new RectangleF(p1.X, p1.Y, 0, 0);
            r2 = new RectangleF(p2.X, p2.Y, 0, 0);
            r1.Inflate(3, 3);
            r2.Inflate(3, 3);
            //this is like a topological neighborhood
            //the connection is shifted left and right
            //and the point under consideration has to be in between.						
            if (RectangleF.Union(r1, r2).Contains(checkPoint))
            {
                if (p1.Y < p2.Y) //SWNE
                {
                    o = r1.Left + (((r2.Left - r1.Left) * (checkPoint.Y - r1.Bottom)) / (r2.Bottom - r1.Bottom));
                    u = r1.Right + (((r2.Right - r1.Right) * (checkPoint.Y - r1.Top)) / (r2.Top - r1.Top));
                    return ((checkPoint.X > o) && (checkPoint.X < u));
                }
                else //NWSE
                {
                    o = r1.Left + (((r2.Left - r1.Left) * (checkPoint.Y - r1.Top)) / (r2.Top - r1.Top));
                    u = r1.Right + (((r2.Right - r1.Right) * (checkPoint.Y - r1.Bottom)) / (r2.Bottom - r1.Bottom));
                    return ((checkPoint.X > o) && (checkPoint.X < u));
                }
            }

            return false;
        }
    }
}