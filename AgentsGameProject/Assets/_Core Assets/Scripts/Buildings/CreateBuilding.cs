using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Constants;

/// <summary>
/// "CreateBuilding" used by building buttons
/// </summary>

public class CreateBuilding : MonoBehaviour
{
    public BuildingType BuildingType;

    GameObject buildingPrefab;

    private ResourcesDataController resourcesDataController;
    
    MapCreator mapCreator;

    float woodCost = 0f;
    float stoneCost = 0f;
    float mineralsCost = 0f;

    float godForceCost = 0f;
    float foodCost = 0f;

    string resourceTag;

    Quaternion rotation = new Quaternion(0, 0, 0, 0);
    GameObject newBuilding = null;
    float lastTimeScale;
    Vector3 v3;
    Vector3 offset;

    bool creatingBuilding;

    GameStates gameStates;
    LevelManager LevelManagerRef;

    void Awake()
    {
        mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();
        resourcesDataController = GameObject.Find("GameManager").GetComponent<ResourcesDataController>();
        gameStates = GameObject.Find("GameManager").GetComponent<GameStates>();

        LevelManagerRef = GameObject.Find("GameManager").GetComponent<LevelManager>();
    }

    void Update()
    {
        if (creatingBuilding)
        {
            DisplayBuildingAtMousePosition();

            SelectLocation();
        }
    }

    // Initialize Building Creation
    // Initialized by a button
    public void CreateBuildingFunction()
    {
        woodCost = 0f;
        stoneCost = 0f;
        mineralsCost = 0f;
        godForceCost = 0f;
        foodCost = 0f;

        GetBuildingPrefabAndCosts();

        // Check costs against avilable resources
        // if True instatiate a Building 
        //and enter "Creating Building" Mode
        if (CheckCosts())
        {
            if(Time.timeScale != 0f) lastTimeScale = Time.timeScale;

            newBuilding = Instantiate(buildingPrefab, new Vector3(0f, 0f, -5f), rotation);
            
            LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.CreatingBuilding]);
            LevelManagerRef.CreateBuildingRef = this;
        }
    }

    public void DisplayBuildingAtMousePosition()
    {
        Time.timeScale = 0f;

        float zOffset = Camera.main.transform.position.z;

        v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        v3 = Camera.main.ScreenToWorldPoint(v3);

        v3 = newBuilding.GetComponent<GenericBuilding>().GridRef.WorldToCell(v3);

        v3.x += 0.5f;
        v3.y += 0.5f;

        newBuilding.transform.position = v3;
    }

    //get user input and check if selected location is valid for that building type
    public void SelectLocation()
    {
        // If left mouse Click
        if (Input.GetMouseButtonDown(0))
        {
            // Check if relevent resource is available at selected location
            // or if location is valid for the relavent type of building
            // If True place building at location
            if (BuildingType.name == "WoodMill" 
                || BuildingType.name == "StoneQuarry" 
                || BuildingType.name == "PowerPlant" 
                || BuildingType.name == "MineralsQuarry")
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                               Camera.main.transform.forward, 15f, LayerMask.GetMask("Resource"));
                if (hit)
                {
                    if (hit.transform.CompareTag(resourceTag))
                    {
                        // Deactivate resource graphics & trigger
                        hit.collider.enabled = false;
                        hit.transform.GetChild(1).gameObject.SetActive(false);
                        hit.transform.GetChild(2).gameObject.SetActive(false);

                        newBuilding.GetComponent<GenericBuilding>().Resource = hit.transform.gameObject.GetComponent<Resource>();

                        PlaceBuilding();
                    }
                }
            }
            else if (BuildingType.name == "BasicFarm")
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                               Camera.main.transform.forward, 15f, LayerMask.GetMask("Ground"));
                if (hit)
                {
                    PlaceBuilding();
                }
            }
            else if (BuildingType.name == "BasicWaterPump")
            {
                Vector3Int v3position = newBuilding.GetComponent<GenericBuilding>().GridRef.WorldToCell(newBuilding.transform.position);

                Vector2 position;

                position.x = v3position.x;
                position.y = v3position.y;

                Collider2D _waterHit = Physics2D.OverlapCircle(position, 1f, LayerMask.GetMask("Water"));
                if (_waterHit)
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                                Camera.main.transform.forward, 15f, LayerMask.GetMask("Ground"));
                    if (hit)
                    {
                        newBuilding.GetComponent<GenericBuilding>().Resource = null;
                        PlaceBuilding();
                    }
                }
            }
        }

        // Cancle building creation
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(newBuilding);
            Time.timeScale = lastTimeScale;

            LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.BaseState]);
            LevelManagerRef.CreateBuildingRef = null;
        }
    }



    void GetBuildingPrefabAndCosts()
    {
        // Get building type & Costs

        buildingPrefab = BuildingType.BuildingPrefab;

        godForceCost = BuildingType.GodForceCost;
        foodCost = BuildingType.FoodCost;
        woodCost = BuildingType.WoodCost;
        stoneCost = BuildingType.StoneCost;
        mineralsCost = BuildingType.MineralCost;

        resourceTag = BuildingType.ResourceTag;
    }
    
    // Place building at selected position
    // Used by SelectLocation() Function
    void PlaceBuilding()
    {
        resourcesDataController.UpdateResourceAmount(GODFORCE, -godForceCost);
        resourcesDataController.UpdateResourceAmount(FOOD, -foodCost);
        resourcesDataController.UpdateResourceAmount(STONE, -stoneCost);
        resourcesDataController.UpdateResourceAmount(WOOD, -woodCost);
        resourcesDataController.UpdateResourceAmount(MINERALS, -mineralsCost);

        Vector3Int position = newBuilding.GetComponent<GenericBuilding>().GridRef.WorldToCell(newBuilding.transform.position);

        position.x += mapCreator.MapWidth / 2;
        position.y += mapCreator.MapHeight / 2;

        newBuilding.GetComponent<GenericBuilding>().LastPosition = position;

        newBuilding.GetComponent<GenericBuilding>().UpdateNode(position, false);
        Time.timeScale = lastTimeScale;

        LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.BaseState]);
        LevelManagerRef.CreateBuildingRef = null;
    }

    bool CheckCosts()
    {
        if (stoneCost <= resourcesDataController.GetResourceAmount(STONE) && woodCost <= resourcesDataController.GetResourceAmount(WOOD)
            && mineralsCost <= resourcesDataController.GetResourceAmount(MINERALS) && godForceCost <= resourcesDataController.GetResourceAmount(GODFORCE)
            && foodCost <= resourcesDataController.GetResourceAmount(FOOD))
        {
            return true;
        }
        else
            return false;
    }
}
