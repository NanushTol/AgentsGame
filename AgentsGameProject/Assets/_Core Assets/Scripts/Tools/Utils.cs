
using UnityEngine;

public class Utils
{
    static public void UpdatePathfinderNode(Vector3Int _position, bool _walkable)
    {
        AstarPath.active.AddWorkItem(ctx => {
            var PfGridGraph = AstarPath.active.data.gridGraph;

            // Mark a single node as unwalkable
            PfGridGraph.GetNode(_position.x, _position.y).Walkable = _walkable;

            // Recalculate the connections for that node as well as its neighbours
            PfGridGraph.CalculateConnectionsForCellAndNeighbours(_position.x, _position.y);
        });
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
