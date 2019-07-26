using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;
using static Constants;

/// <summary>
/// The Basic Building Class
/// all other buildings types MUST use it!
/// its main reason to exist is to have a generic class to communicate with the Agents
/// </summary>

public class GenericBuilding : MonoBehaviour
{
    [HideInInspector]
    public Grid grid;
    
    public BuildingType buildingType;
 
    public Color WorkplaceColor;

    public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    [HideInInspector]
    public ResourcesDataController resourcesDataController;

    [HideInInspector]
    public Vector3Int LastPosition;

    MapCreator mapCreator; 

    [HideInInspector]
    public float WorkEfficiency = 1f;

    //variables accessed by agents
    [HideInInspector]
    public bool WorkersNeeded;
    [HideInInspector]
    public int MaxWorkers;
    [HideInInspector]
    public float Production = 0.0f;
    [HideInInspector]
    public float addedValue;
    [HideInInspector]
    public int AgentsWorking;

    float[] upkeep = new float[6];
    [HideInInspector]
    public bool BuildingWorking;
    [HideInInspector]


    void Awake()
    {
        
        grid = GameObject.Find("Grid").GetComponent<Grid>();

        mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();
        
        resourcesDataController = GameObject.Find("GameManager").GetComponent<ResourcesDataController>();

        transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;
    }


    private void Start()
    {
        // Get Grid Cell Position
        Vector3Int position = grid.WorldToCell(transform.position);

        position.x += mapCreator.MapWidth / 2;
        position.y += mapCreator.MapHeight / 2;

        LastPosition = position;

        // Update Pathfinder Nodes Grid
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
        CheckUpkeep();

        // If the building is working
        if (BuildingWorking)
        {
            // check if workers are needed
            if (AgentsWorking <= MaxWorkers)
            {

                WorkersNeeded = true;

                if (AgentsWorking == MaxWorkers)
                {
                    WorkersNeeded = false;
                }

            }
            if (AgentsWorking > MaxWorkers)
            {
                AgentsWorking = MaxWorkers;
                WorkersNeeded = false;
            }


            UpdateVacancyBar(AgentsWorking);


            if (Production > 0)
            {
                addedValue = Production;
            }

        }

        //update that workers are not needed if the building dosent work
        else if (BuildingWorking == false)
        {
            WorkersNeeded = false;
        }
    }

    // Updates the Pathfinder Nodes Grid
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

    void UpdateVacancyBar(int _agentsWorking)
    {
        Transform vacancyBar = transform.GetChild(0);

        for (int j = 0; j < MaxWorkers; j++)
        {
            vacancyBar.transform.GetChild(j).gameObject.GetComponent<SpriteRenderer>().color = NotWorkingColor;
        }

        for (int i = 0; i < _agentsWorking; i++)
        {
            vacancyBar.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = WorkingColor;
        }

    }

    //Checks to see if there are enought resources to run the building
    void CheckUpkeep()
    {
        //check Upkeep to see if building can work
        if (resourcesDataController.GetResourceAmount(GODFORCE) >= buildingType.GodForceUpkeep &&
            resourcesDataController.GetResourceAmount(ENERGY) >= buildingType.EnergyUpkeep &&
            resourcesDataController.GetResourceAmount(RESEARCH) >= buildingType.ResearchUpkeep &&
            resourcesDataController.GetResourceAmount(FOOD) >= buildingType.FoodUpkeep &&
            resourcesDataController.GetResourceAmount(WATER) >= buildingType.WaterUpkeep &&
            resourcesDataController.GetResourceAmount(STONE) >= buildingType.StoneUpkeep &&
            resourcesDataController.GetResourceAmount(WOOD) >= buildingType.WoodUpkeep &&
            resourcesDataController.GetResourceAmount(MINERALS) >= buildingType.MineralUpkeep)
        {
            UpdateUpkeep();

            BuildingWorking = true;
            transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;
        }

        //update Building Color & state
        else
        {
            BuildingWorking = false;
            transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    //Updates the resources production acording to the building's upkeep
    void UpdateUpkeep()
    {
        // subtract upkeep from resources
        resourcesDataController.UpdateResourceProduction(GODFORCE, (-buildingType.GodForceUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(ENERGY, (-buildingType.EnergyUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(RESEARCH, (-buildingType.ResearchUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(FOOD, (-buildingType.FoodUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(WATER, (-buildingType.WaterUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(STONE, (-buildingType.StoneUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(WOOD, (-buildingType.WoodUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(MINERALS, (-buildingType.MineralUpkeep * Time.deltaTime));
    }
}
