using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public class GenericBuilding : MonoBehaviour
{
    [HideInInspector]
    public Grid grid;

    public enum TypeOfWorkplace { BasicFarm, WoodMill}
    public TypeOfWorkplace typeOfWorkplace;

    [HideInInspector]
    public Vector3Int LastPosition;

    public Color WorkplaceColor;

    MapCreator mapCreator;

    [HideInInspector]
    public float WorkEfficiency;

    //variables accessed by agents
    public bool WorkersNeeded;
    public float Production = 0.0f;



    // Start is called before the first frame update
    void Awake()
    {
        //environment = GameObject.Find("Environment").GetComponent<Environment>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();

        transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;

        #region //switch
        /*
        switch (typeOfWorkplace)
        {
            case TypeOfWorkplace.BasicFarm:
                gameObject.AddComponent<BasicFarm>();
                transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;
                break;

            case TypeOfWorkplace.WoodMill:
                gameObject.AddComponent<WoodMill>();
                transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;
                break;
        }*/
        #endregion
    }
    private void Start()
    {
        Vector3Int position = grid.WorldToCell(transform.position);

        position.x += mapCreator.MapWidth / 2;
        position.y += mapCreator.MapHeight / 2;

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
