using System.Collections.Generic;

namespace Pathfinding
{
    public interface IPathNode
    {
        IEnumerable<IPathNode> Neighbors { get; }
        bool IsWalkable { get; }
    }
}