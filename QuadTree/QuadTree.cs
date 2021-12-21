using System.Collections.Generic;

public class QuadTree<T> where T : IQuadPoint
{
    public QuadTree(QuadRegion maxRegion, int maxPoints, int maxDepth)
    {
        Root = new QuadNode<T>(this, null, maxRegion, Quadrant.None, 0);
        MaxPoints = maxPoints;
        MaxDepth = maxDepth;
    }

    public void Update()
    {
        Queue<QuadNode<T>> queue = new Queue<QuadNode<T>>();
        queue.Enqueue(Root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            node.Update();

            if (!node.IsLeaf)
            {
                foreach (var child in node.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
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