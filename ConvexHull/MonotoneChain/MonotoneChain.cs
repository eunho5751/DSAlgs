using System;
using System.Collections.Generic;
using System.Linq;

namespace ConvexHull
{
    public class MonotoneChain
    {
        public Point2[] Execute(IEnumerable<Point2> points)
        {
            var sortedPoints = points.ToArray();
            if (sortedPoints.Length > 1)
            {
                // First, Sort the points lexicographically.
                // (first by x-coord, and in case of a tie, by y-coord)
                Array.Sort(sortedPoints, (p1, p2) => (p1.X < p2.X) ? -1 : ((p1.X == p2.X) ? ((p1.Y < p2.Y) ? -1 : 1) : 1));

                int count = sortedPoints.Length;
                int k = 0;
                Point2[] pts = new Point2[count * 2];

                // Lower hull
                for (int i = 0; i < count; i++)
                {
                    // When the next point is counter-clockwise turn, it is not the convex-hull point. 
                    while (k >= 2 && Point2.Cross(pts[k - 1] - pts[k - 2], sortedPoints[i] - pts[k - 2]) <= 0)
                    {
                        k--;
                    }

                    pts[k++] = sortedPoints[i];
                }

                // Upper hull
                for (int i = count - 2, t = k + 1; i >= 0; i--)
                {
                    while (k >= t && Point2.Cross(pts[k - 1] - pts[k - 2], sortedPoints[i] - pts[k - 2]) <= 0)
                    {
                        k--;
                    }

                    pts[k++] = sortedPoints[i];
                }

                // (k-1)th index point is excluded because it is a duplicate of 0th index point.
                Array.Resize(ref pts, k - 1);
                return pts;
            }
            else
            {
                return sortedPoints;
            }
        }
    }
}
