using System;
using System.Collections.Generic;

namespace Pathfinding
{
    public class Dijkstra<TNode> where TNode : IPathNode
    {
        private class Edge : IComparable<Edge>
        {
            public Edge(Edge parent, TNode target, float totalCost)
            {
                Parent = parent;
                Target = target;
                TotalCost = totalCost;
            }

            public int CompareTo(Edge other)
            {
                return TotalCost.CompareTo(other.TotalCost);
            }

            public Edge Parent { get; }
            public TNode Target { get; }
            public float TotalCost { get; }
        }

        private readonly IPathGraph<TNode> _graph;

        public Dijkstra(IPathGraph<TNode> graph)
        {
            _graph = graph;
        }

        public IEnumerable<TNode> FindPath(TNode start, TNode end)
        {
            Heap<Edge> priorityQueue = new Heap<Edge>(HeapType.Min);
            HashSet<TNode> visitedSet = new HashSet<TNode>();
            List<TNode> pathResult = new List<TNode>();

            priorityQueue.Push(new Edge(null, start, 0));

            Edge toEdge = null;
            while (!priorityQueue.IsEmpty())
            {
                Edge edge = priorityQueue.Pop();
                TNode curNode = edge.Target;
                float curCost = edge.TotalCost;

                if (visitedSet.Contains(curNode))
                    continue;

                if (curNode.Equals(end))
                {
                    toEdge = edge;
                    break;
                }

                var neighbors = curNode.Neighbors;
                foreach (TNode nextNode in neighbors)
                {
                    if (visitedSet.Contains(nextNode) || !nextNode.IsWalkable)
                        continue;

                    float nextCost = curCost + _graph.GetCost(curNode, nextNode);
                    priorityQueue.Push(new Edge(edge, nextNode, nextCost));
                }

                visitedSet.Add(curNode);
            }

            // Backtracking
            if (toEdge != null)
            {
                pathResult.Add(toEdge.Target);
                while (toEdge.Parent != null)
                {
                    pathResult.Add(toEdge.Parent.Target);
                    toEdge = toEdge.Parent;
                }
                pathResult.Reverse();
            }

            return pathResult;
        }
    }
}