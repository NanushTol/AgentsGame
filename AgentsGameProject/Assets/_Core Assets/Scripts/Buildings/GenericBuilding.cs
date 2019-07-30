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
    
    public BuildingType BuildingType;
 
    public Color WorkplaceColor;

    public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    [HideInInspector]
    public ResourcesDataController resourcesDataController;

    [HideInInspector]
    public Vector3Int LastPosition;

    MapCreator mapCreator; 

    [HideInInspector]
    public float ProductionRate;

    //variables accessed by agents
    //[HideInInspector]
    public bool WorkersNeeded;
    [HideInInspector]
    public int MaxWorkers;
    [HideInInspector]
    public float Production = 0.0f;
    [HideInInspector]
    public float addedValue;
    //[HideInInspector]
    public int AgentsWorking;

    float[] upkeep = new float[6];
    [HideInInspector]
    public bool UpkeepValid;
    [HideInInspector]
    public bool BuildingActive = true;
    [HideInInspector]
    public bool UserOverrideBuildingActive = true;

    [HideInInspector]
    public Resource Resource;
    [HideInInspector]
    public float ResourceAmount;



    void Awake()
    {
        
        grid = GameObject.Find("Grid").GetComponent<Grid>();

        mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();
        
        resourcesDataController = GameObject.Find("GameManager").GetComponent<ResourcesDataController>();

        transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;

        UserOverrideBuildingActive = true;

        
}


    void Start()
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
        if (Resource != null) ResourceAmount = Resource.Amount;

        CheckUpkeep();

        // If the building is working
        if (UpkeepValid && BuildingActive)
        {
            // check if workers are needed
            if (AgentsWorking < MaxWorkers)
            {
                WorkersNeeded = true;
            }
            else if (AgentsWorking >= MaxWorkers)
            {
                WorkersNeeded = false;
            }


            UpdateVacancyBar(AgentsWorking);


            if (Production > 0)
            {
                addedValue = Production;

                if (Resource != null) Resource.Amount -= Production;
            }

        }

        //update that workers are not needed if the building dosent work
        else
        {
            WorkersNeeded = false;
            UpdateVacancyBar(AgentsWorking);
            transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
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

    void UpdateVacancyBar(int agentsWorking)
    {
        Transform vacancyBar = transform.GetChild(0);

        for (int j = 0; j < MaxWorkers; j++)
        {
            vacancyBar.transform.GetChild(j).gameObject.GetComponent<SpriteRenderer>().color = NotWorkingColor;
        }

        for (int i = 0; i < agentsWorking; i++)
        {
            vacancyBar.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = WorkingColor;
        }

    }

    //Checks to see if there are enought resources to run the building
    void CheckUpkeep()
    {
        if (BuildingType.name == "BasicFarm")
        {
            Food food = GetComponent<Food>();

            if (food.FoodValue >= food.MaxFood)
            {
                UpkeepValid = false;
                BuildingActive = false;
                return;
            }
        }

        //check Upkeep to see if building can work
        if (resourcesDataController.GetResourceAmount(GODFORCE) >= BuildingType.GodForceUpkeep &&
            resourcesDataController.GetResourceAmount(ENERGY) >= BuildingType.EnergyUpkeep &&
            resourcesDataController.GetResourceAmount(RESEARCH) >= BuildingType.ResearchUpkeep &&
            resourcesDataController.GetResourceAmount(FOOD) >= BuildingType.FoodUpkeep &&
            resourcesDataController.GetResourceAmount(WATER) >= BuildingType.WaterUpkeep &&
            resourcesDataController.GetResourceAmount(STONE) >= BuildingType.StoneUpkeep &&
            resourcesDataController.GetResourceAmount(WOOD) >= BuildingType.WoodUpkeep &&
            resourcesDataController.GetResourceAmount(MINERALS) >= BuildingType.MineralUpkeep)
        {
            UpdateUpkeep();

            UpkeepValid = true;

            if(UserOverrideBuildingActive) BuildingActive = true;
            else if(UserOverrideBuildingActive == false) BuildingActive = false;

            transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = WorkplaceColor;
        }

        //update Building Color & state
        else
        {
            UpkeepValid = false;
            BuildingActive = false;
            transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    //Updates the resources production acording to the building's upkeep
    void UpdateUpkeep()
    {
        // subtract upkeep from resources
        resourcesDataController.UpdateResourceProduction(GODFORCE, (-BuildingType.GodForceUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(ENERGY, (-BuildingType.EnergyUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(RESEARCH, (-BuildingType.ResearchUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(FOOD, (-BuildingType.FoodUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(WATER, (-BuildingType.WaterUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(STONE, (-BuildingType.StoneUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(WOOD, (-BuildingType.WoodUpkeep * Time.deltaTime));
        resourcesDataController.UpdateResourceProduction(MINERALS, (-BuildingType.MineralUpkeep * Time.deltaTime));
    }
}
