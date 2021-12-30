
namespace Pathfinding
{
    public interface IPathGraph<TNode> where TNode : IPathNode
    {
        float GetCost(TNode from, TNode to);
    }
}