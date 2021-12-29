using System;
using System.Collections.Generic;

public abstract class QuadNode<TNode, TPoint> where TNode : QuadNode<TNode, TPoint> where TPoint : IQuadPoint
{
    private readonly TNode[] _children;
    private readonly List<TPoint> _points;
    private readonly List<TNode> _neighbors;

    public QuadNode(QuadTree<TNode, TPoint> tree, TNode parent, QuadRegion region)
    {
        _children = new TNode[4];
        _points = new List<TPoint>();
        _neighbors = new List<TNode>();
        
        Tree = tree;
        Parent = parent;
        Region = region;
        Quadrant = CalcQuadrant();
        Depth = parent != null ? parent.Depth + 1 : 0;
    }

    public void Update()
    {
        if (Quadrant == Quadrant.None)
            return;

        _neighbors.Clear();
        SearchNeighbors(_neighbors, Quadrant.NW, Quadrant.NE, true);
        SearchNeighbors(_neighbors, Quadrant.SW, Quadrant.SE, true);
        SearchNeighbors(_neighbors, Quadrant.NW, Quadrant.SW, false);
        SearchNeighbors(_neighbors, Quadrant.NE, Quadrant.SE, false);
    }

    public TNode GetChild(Quadrant quadrant)
    {
        return quadrant switch
        {
            Quadrant.SW => _children[0],
            Quadrant.SE => _children[1],
            Quadrant.NE => _children[2],
            Quadrant.NW => _children[3],
            _ => throw new InvalidOperationException("Can't use quadrant other than 'SW, SE, NE, NW'."),
        };
    }

    public TNode GetNode(Point2 position)
    {
        if (InRegion(position))
        {
            if (IsLeaf)
            {
                return (TNode)this;
            }
            else
            {
                foreach (var child in _children)
                {
                    if (child.InRegion(position))
                    {
                        return child.GetNode(position);
                    }
                }

                return null;
            }
        }

        return null;
    }

    public bool InRegion(Point2 position)
    {
        return Region.Contains(position);
    }

    public void Insert(TPoint point)
    {
        if (!point.InRegion(Region))
            return;

        if (IsLeaf)
        {
            _points.Add(point);
            if ((Tree.MaxPoints > 0 && _points.Count >= Tree.MaxPoints) && Depth < Tree.MaxDepth)
            {
                Subdivide();

                foreach (TPoint p in _points)
                {
                    InsertInChild(p);
                }
                _points.Clear();
            }
        }
        else
        {
            InsertInChild(point);
        }
    }

    private void InsertInChild(TPoint point)
    {
        foreach (var child in _children)
        {
            child.Insert(point);
        }
    }

    private void Subdivide()
    {
        Point2 position = Region.Position;
        float halfW = Region.Width / 2f;
        float halfH = Region.Height / 2f;
        
        _children[0] = Tree.CreateNode(Tree, (TNode)this, new QuadRegion(position, halfW, halfH));
        _children[1] = Tree.CreateNode(Tree, (TNode)this, new QuadRegion(new Point2(position.X + halfW, position.Y), halfW, halfH));
        _children[2] = Tree.CreateNode(Tree, (TNode)this, new QuadRegion(new Point2(position.X + halfW, position.Y + halfH), halfW, halfH));
        _children[3] = Tree.CreateNode(Tree, (TNode)this, new QuadRegion(new Point2(position.X, position.Y + halfH), halfW, halfH));
    }

    private void SearchNeighbors(List<TNode> neighbors, Quadrant quadrant1, Quadrant quadrant2, bool vertical)
    {
        Quadrant flippedQuadrant1 = vertical ? FlipQuadarantVertically(quadrant1) : FlipQuadrantHorizontally(quadrant1);
        Quadrant flippedQuadrant2 = vertical ? FlipQuadarantVertically(quadrant2) : FlipQuadrantHorizontally(quadrant2);

        if (Quadrant == flippedQuadrant1)
        {
            var child = Parent.GetChild(quadrant1);
            if (child.IsLeaf)
            {
                neighbors.Add(child);
            }
            else
            {
                GetDeepestNodes(neighbors, child, flippedQuadrant1, flippedQuadrant2);
            }
        }
        else if (Quadrant == flippedQuadrant2)
        {
            var child = Parent.GetChild(quadrant2);
            if (child.IsLeaf)
            {
                neighbors.Add(child);
            }
            else
            {
                GetDeepestNodes(neighbors, child, flippedQuadrant1, flippedQuadrant2);
            }
        }
        else
        {
            Stack<Quadrant> backTracker = new Stack<Quadrant>();

            var cur = this;
            while (cur != Tree.Root)
            {
                backTracker.Push(cur.Quadrant);
                if (cur.Quadrant == flippedQuadrant1 || cur.Quadrant == flippedQuadrant2)
                    break;

                cur = cur.Parent;
            }

            if (cur != Tree.Root)
            {
                TNode child = cur.Parent;
                while (backTracker.Count > 0)
                {
                    Quadrant quadrant = backTracker.Pop();
                    quadrant = vertical ? FlipQuadarantVertically(quadrant) : FlipQuadrantHorizontally(quadrant);
                    child = child.GetChild(quadrant);

                    if (child.IsLeaf)
                        break;
                }

                if (child.IsLeaf)
                {
                    neighbors.Add(child);
                }
                else
                {
                    GetDeepestNodes(neighbors, child, flippedQuadrant1, flippedQuadrant2);
                }
            }
        }
    }

    private void GetDeepestNodes(List<TNode> ret, TNode node, Quadrant quadrant1, Quadrant quadrant2)
    {
        var child1 = node.GetChild(quadrant1);
        if (child1.IsLeaf)
        {
            ret.Add(child1);
        }
        else
        {
            GetDeepestNodes(ret, child1, quadrant1, quadrant2);
        }

        var child2 = node.GetChild(quadrant2);
        if (child2.IsLeaf)
        {
            ret.Add(child2);
        }
        else
        {
            GetDeepestNodes(ret, child2, quadrant1, quadrant2);
        }
    }

    private Quadrant FlipQuadrantHorizontally(Quadrant quadrant)
    {
        return quadrant switch
        {
            Quadrant.SW => Quadrant.SE,
            Quadrant.SE => Quadrant.SW,
            Quadrant.NE => Quadrant.NW,
            Quadrant.NW => Quadrant.NE
        };
    }

    private Quadrant FlipQuadarantVertically(Quadrant quadrant)
    {
        return quadrant switch
        {
            Quadrant.SW => Quadrant.NW,
            Quadrant.SE => Quadrant.NE,
            Quadrant.NE => Quadrant.SE,
            Quadrant.NW => Quadrant.SW
        };
    }

    private Quadrant CalcQuadrant()
    {
        if (Parent != null)
        {
            Point2 parentPos = Parent.Region.Position;
            float pcx = parentPos.X + Parent.Region.Width / 2f;
            float pcy = parentPos.Y + Parent.Region.Height / 2f;

            Point2 myPos = Region.Position;
            float cx = myPos.X + Region.Width / 2f;
            float cy = myPos.Y + Region.Height / 2f;

            if (cx < pcx && cy < pcy) return Quadrant.SW;
            else if (cx > pcx && cy < pcy) return Quadrant.SE;
            else if (cx > pcx && cy > pcy) return Quadrant.NE;
            else if (cx < pcx && cy > pcy) return Quadrant.NW;
            else return Quadrant.None;
        }
        else
        {
            return Quadrant.None;
        }
    }

    public QuadTree<TNode, TPoint> Tree { get; }

    public TNode Parent { get; }

    public QuadRegion Region { get; }

    public Quadrant Quadrant { get; }

    public int Depth { get; }

    public bool IsLeaf => _children[0] == null;

    public bool HasPoints => _points.Count > 0;

    public IEnumerable<TNode> Children => _children;

    public IEnumerable<TNode> Neighbors => _neighbors;

    public IReadOnlyCollection<TPoint> Points => _points;
}
