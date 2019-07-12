using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public class GenericBuilding : MonoBehaviour
{
    [HideInInspector]
    public Grid grid;

    public enum TypeOfWorkplace { BasicFarm, WoodMill }
    public TypeOfWorkplace typeOfWorkplace;

    [HideInInspector]
    public Vector3Int LastPosition;

    public Color WorkplaceColor;

    MapCreator mapCreator;
    CostsUpkeepProductionData cupData;
    ResourcesData resourcesData;

    [HideInInspector]
    public float WorkEfficiency;

    //variables accessed by agents
    [HideInInspector]
    public bool WorkersNeeded;
    [HideInInspector]
    public float Production = 0.0f;
    [HideInInspector]
    public int AgentsWorking;

    float[] upkeep = new float[6];
    [HideInInspector]
    public bool BuildingWorking;

    // Start is called before the first frame update
    void Awake()
    {
        //environment = GameObject.Find("Environment").GetComponent<Environment>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();
        cupData = GameObject.Find("GameManager").GetComponent<CostsUpkeepProductionData>();
        resourcesData = GameObject.Find("GameManager").GetComponent<ResourcesData>();

        transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;

        switch (typeOfWorkplace)
        {
            case TypeOfWorkplace.BasicFarm:
                upkeep[0] = 0f; // GodForce
                upkeep[1] = cupData.BasicFarmEnergyUpkeep; // Energy
                upkeep[2] = cupData.BasicFarmWaterUpkeep; // Water
                upkeep[3] = 0f; // Stone
                upkeep[4] = 0f; // Wood
                upkeep[5] = cupData.BasicFarmMineralsUpkeep; // Minerals
                break;

            case TypeOfWorkplace.WoodMill:
                upkeep[0] = 0f; // GodForce
                upkeep[1] = cupData.WoodMillEnergyUpkeep; // Energy
                upkeep[2] = cupData.WoodMillWaterUpkeep; // Water
                upkeep[3] = 0f; // Stone
                upkeep[4] = 0f; // Wood
                upkeep[5] = 0f; // Minerals
                break;
        }

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

    void Update()
    {
        if (resourcesData.GodForceAmount > upkeep[0] &&
            resourcesData.EnergyProduction > upkeep[1] &&
            resourcesData.WaterProduction > upkeep[2] &&
            resourcesData.StoneAmount > upkeep[3] &&
            resourcesData.WoodAmount > upkeep[4] &&
            resourcesData.MineralsAmount > upkeep[5])
        {
            // subtract upkeep from resources
            resourcesData.GodForceAmount -= upkeep[0] * Time.deltaTime;
            resourcesData.EnergyProduction -= upkeep[1] * Time.deltaTime;
            resourcesData.WaterProduction -= upkeep[2] * Time.deltaTime;
            resourcesData.StoneAmount -= upkeep[3] * Time.deltaTime;
            resourcesData.WoodAmount -= upkeep[4] * Time.deltaTime;
            resourcesData.MineralsAmount -= upkeep[5] * Time.deltaTime;

            BuildingWorking = true;
            transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;
        }
        else
        {
            BuildingWorking = false;
            transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        
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
