using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public class WorkPlace : MonoBehaviour
{
    public Grid grid;

    public enum TypeOfWorkplace { BasicFarm, Coller, Top, Bottom }
    public TypeOfWorkplace typeOfWorkplace;

    public bool WorkersNeeded;

    [HideInInspector]
    public Vector3Int LastPosition;

    public float WorkEfficiency;
    public float Production = 0.0f;

    public Color WorkplaceColor;

    // Start is called before the first frame update
    void Awake()
    {
        //environment = GameObject.Find("Environment").GetComponent<Environment>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();


        switch (typeOfWorkplace)
        {
            case TypeOfWorkplace.BasicFarm:
                gameObject.AddComponent<BasicFarm>();
                transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;
                break;
        }
    }
    private void Start()
    {
        Vector3Int position = grid.WorldToCell(transform.position);

        position.x += 18;
        position.y += 17;

        LastPosition = position;

        var gg = AstarPath.active.data.gridGraph;
        int x = position.x;
        int y = position.y;
        GridNodeBase node = gg.GetNode(x, y);

        AstarPath.active.AddWorkItem(ctx => {
            var PfGridGraph = AstarPath.active.data.gridGraph;

            // Mark a single node as unwalkable
            PfGridGraph.GetNode(x, y).Walkable = false;

            // Recalculate the connections for that node as well as its neighbours
            PfGridGraph.CalculateConnectionsForCellAndNeighbours(x, y);
        });

        //transform.GetChild(1).localScale = new Vector3(GrowingRadius * 2f, 0.1f, GrowingRadius * 2f);
    }


    public void UpdateNode(Vector3Int _position, bool _walkable)
    {
        AstarPath.active.AddWorkItem(ctx => {
            var PfGridGraph = AstarPath.active.data.gridGraph;

            // Mark a single node as unwalkable
            PfGridGraph.GetNode(_position.x, _position.y).Walkable = _walkable;

            // Recalculate the connections for that node as well as its neighbours
            PfGridGraph.CalculateConnectionsForCellAndNeighbours(_position.x, _position.y);
        });
    }

    
}
