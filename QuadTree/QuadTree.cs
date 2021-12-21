using System;

public class QuadTree<T> where T : IQuadPoint
{
    public QuadTree(QuadRegion maxRegion, int maxPoints, int maxDepth)
    {
        Root = new QuadNode<T>(this, null, maxRegion, Quadrant.None, 0);
        MaxPoints = maxPoints;
        MaxDepth = maxDepth;
    }

    public void Insert(T point)
    {
        Root.Insert(point);
    }

    public QuadNode<T> GetNode(float x, float y)
    {
        return Root.GetNode(x, y);
    }

    public QuadNode<T> Root { get; }

    public int MaxPoints { get; }

    public int MaxDepth { get; }
}