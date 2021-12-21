using System;
using System.Collections.Generic;

public class QuadNode<T> where T : IQuadPoint
{
    private readonly QuadNode<T>[] _children;
    private readonly List<T> _points;
    private readonly List<QuadNode<T>> _neighbors;

    public QuadNode(QuadTree<T> tree, QuadNode<T> parent, QuadRegion region, Quadrant quadrant, int depth)
    {
        _children = new QuadNode<T>[4];
        _points = new List<T>();
        _neighbors = new List<QuadNode<T>>();

        Tree = tree;
        Parent = parent;
        Region = region;
        Quadrant = quadrant;
        Depth = depth;
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

    public QuadNode<T> GetChild(Quadrant quadrant)
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

    public QuadNode<T> GetNode(float x, float y)
    {
        if (InRegion(x, y))
        {
            if (IsLeaf)
            {
                return this;
            }
            else
            {
                foreach (var child in _children)
                {
                    if (child.InRegion(x, y))
                    {
                        return child.GetNode(x, y);
                    }
                }

                return null;
            }
        }

        return null;
    }

    public bool InRegion(float x, float y)
    {
        return Region.Contains(x, y);
    }

    public void Insert(T point)
    {
        if (!point.InRegion(Region))
            return;

        if (IsLeaf)
        {
            _points.Add(point);
            if ((Tree.MaxPoints > 0 && _points.Count >= Tree.MaxPoints) && Depth < Tree.MaxDepth)
            {
                Subdivide();

                foreach (T p in _points)
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

    private void InsertInChild(T point)
    {
        foreach (var child in _children)
        {
            child.Insert(point);
        }
    }

    private void Subdivide()
    {
        float x = Region.X;
        float y = Region.Y;
        float halfW = Region.Width / 2f;
        float halfH = Region.Height / 2f;

        _children[0] = new QuadNode<T>(Tree, this, new QuadRegion(x, y, halfW, halfH), Quadrant.SW, Depth + 1);
        _children[1] = new QuadNode<T>(Tree, this, new QuadRegion(x + halfW, y, halfW, halfH), Quadrant.SE, Depth + 1);
        _children[2] = new QuadNode<T>(Tree, this, new QuadRegion(x + halfW, y + halfH, halfW, halfH), Quadrant.NE, Depth + 1);
        _children[3] = new QuadNode<T>(Tree, this, new QuadRegion(x, y + halfH, halfW, halfH), Quadrant.NW, Depth + 1);
    }

    private void SearchNeighbors(List<QuadNode<T>> neighbors, Quadrant quadrant1, Quadrant quadrant2, bool vertical)
    {
        Quadrant flippedQuadrant1 = vertical ? FlipQuadarantVertically(quadrant1) : FlipQuadrantHorizontally(quadrant1);
        Quadrant flippedQuadrant2 = vertical ? FlipQuadarantVertically(quadrant2) : FlipQuadrantHorizontally(quadrant2);

        if (Quadrant == flippedQuadrant1)
        {
            var child = Parent.GetChild(quadrant1);
            neighbors.Add(child);
        }
        else if (Quadrant == flippedQuadrant2)
        {
            var child = Parent.GetChild(quadrant2);
            neighbors.Add(child);
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
                QuadNode<T> child = cur.Parent;
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

    private void GetDeepestNodes(List<QuadNode<T>> ret, QuadNode<T> node, Quadrant quadrant1, Quadrant quadrant2)
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

    public QuadTree<T> Tree { get; }

    public QuadNode<T> Parent { get; }

    public QuadRegion Region { get; }

    public Quadrant Quadrant { get; }

    public int Depth { get; }

    public bool IsLeaf => _children[0] == null;

    public IEnumerable<QuadNode<T>> Children => _children;

    public IEnumerable<QuadNode<T>> Neighbors => _neighbors;
}
