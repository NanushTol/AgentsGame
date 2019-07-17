using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public class GenericBuilding : MonoBehaviour
{
    [HideInInspector]
    public Grid grid;

    public enum TypeOfWorkplace { BasicFarm, WoodMill, StoneQuarry, PowerPlant, BasicWaterPump}
    public TypeOfWorkplace typeOfWorkplace;

    [HideInInspector]
    public Vector3Int LastPosition;

    public Color WorkplaceColor;

    public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    MapCreator mapCreator;
    CostsUpkeepProductionData cupData;
    ResourcesData resourcesData;

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
    public Component BuildingScript;

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

            case TypeOfWorkplace.StoneQuarry:
                upkeep[0] = 0f; // GodForce
                upkeep[1] = cupData.StoneQuarryEnergyUpkeep; // Energy
                upkeep[2] = cupData.StoneQuarryWaterUpkeep; // Water
                upkeep[3] = 0f; // Stone
                upkeep[4] = 0f; // Wood
                upkeep[5] = 0f; // Minerals
                break;

            case TypeOfWorkplace.PowerPlant:
                upkeep[0] = 0f; // GodForce
                upkeep[1] = 0f; // Energy
                upkeep[2] = cupData.PowerPlantWaterUpkeep; // Water
                upkeep[3] = 0f; // Stone
                upkeep[4] = 0f; // Wood
                upkeep[5] = 0f; // Minerals
                break;

            case TypeOfWorkplace.BasicWaterPump:
                upkeep[0] = 0f; // GodForce
                upkeep[1] = cupData.BasicWaterPumpEnergyUpkeep; // Energy
                upkeep[2] = 0f; // Water
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
        //check Upkeep to see if building can work
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

        //update Building Color & state
        else
        {
            BuildingWorking = false;
            transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }


        // If the building is working
        if (BuildingWorking)
        {
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
}
