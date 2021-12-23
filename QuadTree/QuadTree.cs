using System.Collections.Generic;

public delegate TNode QuadNodeCreator<TNode, TPoint>(QuadTree<TNode, TPoint> tree, TNode parent, QuadRegion region) where TNode : QuadNode<TNode, TPoint> where TPoint : IQuadPoint;

public class QuadTree<TNode, TPoint> where TNode : QuadNode<TNode, TPoint> where TPoint : IQuadPoint
{
    private QuadNodeCreator<TNode, TPoint> _nodeCreator;

    public QuadTree(QuadNodeCreator<TNode, TPoint> nodeCreator, QuadRegion maxRegion, int maxPoints, int maxDepth)
    {
        _nodeCreator = nodeCreator;

        Root = nodeCreator(this, null, maxRegion);
        MaxPoints = maxPoints;
        MaxDepth = maxDepth;
    }

    public void Update()
    {
        Queue<TNode> queue = new Queue<TNode>();
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

    public void Insert(TPoint point)
    {
        Root.Insert(point);
    }

    public TNode GetNode(float x, float y)
    {
        return Root.GetNode(x, y);
    }

    public TNode CreateNode(QuadTree<TNode, TPoint> tree, TNode parent, QuadRegion region)
    {
        return _nodeCreator(tree, parent, region);
    }

    public TNode Root { get; }

    public int MaxPoints { get; }

    public int MaxDepth { get; }
}