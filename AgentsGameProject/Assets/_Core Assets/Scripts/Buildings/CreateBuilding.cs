using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateBuilding : MonoBehaviour
{
    public enum BuildingType { WoodMill, BasicFarm}
    public BuildingType buildingType;

    GameObject buildingPrefab;

    CostsUpkeepProductionData cupData;
    ResourcesData resourcesData;
    MapCreator mapCreator;

    float woodCost = 0f;
    float stoneCost = 0f;
    float mineralsCost = 0f;

    float godForceCost = 0f;
    float foodCost = 0f;

    string tag;

    Quaternion rotation = new Quaternion(0, 0, 0, 0);
    GameObject newBuilding = null;
    float lastTimeScale;
    Vector3 v3;
    float distance;
    Vector3 offset;

    bool creatingBuilding;

    void Awake()
    {
        cupData = GameObject.Find("GameManager").GetComponent<CostsUpkeepProductionData>();
        resourcesData = GameObject.Find("GameManager").GetComponent<ResourcesData>();
        mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();
    }

    void Update()
    {
        if (creatingBuilding)
        {
            PositionBuilding();
        }
    }


    public void CreateBuildingFunction()
    {
        woodCost = 0f;
        stoneCost = 0f;
        mineralsCost = 0f;
        godForceCost = 0f;
        foodCost = 0f;

        GetBuildingPrefabAndCosts();

        // Check costs against avilable resources
        if (stoneCost <= resourcesData.StoneAmount && woodCost <= resourcesData.WoodAmount
            && mineralsCost <= resourcesData.MineralsAmount && godForceCost <= resourcesData.GodForceAmount 
            && foodCost <=resourcesData.FoodAmount)
        {
            lastTimeScale = Time.timeScale;
            creatingBuilding = true;
            newBuilding = Instantiate(buildingPrefab, new Vector3(0f, 0f, -5f), rotation);
        }
    }

    void PositionBuilding()
    {
        Time.timeScale = 0f;

        float zOffset = Camera.main.transform.position.z;

        v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);

        v3 = Camera.main.ScreenToWorldPoint(v3);

        v3 = newBuilding.GetComponent<GenericBuilding>().grid.WorldToCell(v3);

        v3.x += 0.5f;
        v3.y += 0.5f;

        newBuilding.transform.position = v3;


        // left mouse Click
        if (Input.GetMouseButtonDown(0))
        {
            if (buildingType == BuildingType.WoodMill)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                               Camera.main.transform.forward, 15f, LayerMask.GetMask("Resource"));

                if (hit)
                {
                    if (hit.transform.CompareTag(tag))
                    {
                        hit.collider.enabled = false;
                        hit.transform.GetChild(1).gameObject.SetActive(false);
                        hit.transform.GetChild(2).gameObject.SetActive(false);

                        PlaceBuilding();
                    }
                }
            }
            else if (buildingType == BuildingType.BasicFarm)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                               Camera.main.transform.forward, 15f, LayerMask.GetMask("Ground"));
                if (hit.transform.CompareTag(tag))
                {
                    PlaceBuilding();
                }
            }

            
        }

        // Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(newBuilding);
            Time.timeScale = lastTimeScale;
            creatingBuilding = false;
        }
    }

    void GetBuildingPrefabAndCosts()
    {
        // Get building type & Get Costs
        switch (buildingType)
        {
            case BuildingType.WoodMill:
                woodCost = cupData.WoodMillWoodCost;
                stoneCost = cupData.WoodMillStoneCost;
                tag = "Wood";
                buildingPrefab = AssetDatabase.LoadAssetAtPath("Assets/_Core Assets/Prefabs/Buildings/WoodMill.prefab", typeof(Object)) as GameObject;
                break;
        }
    }

    void PlaceBuilding()
    {
        resourcesData.StoneAmount -= stoneCost;
        resourcesData.WoodAmount -= woodCost;
        resourcesData.MineralsAmount -= mineralsCost;
        resourcesData.FoodAmount -= foodCost;
        resourcesData.GodForceAmount -= godForceCost;

        Vector3Int position = newBuilding.GetComponent<GenericBuilding>().grid.WorldToCell(newBuilding.transform.position);

        position.x += mapCreator.MapWidth / 2;
        position.y += mapCreator.MapHeight / 2;

        newBuilding.GetComponent<GenericBuilding>().LastPosition = position;

        newBuilding.GetComponent<GenericBuilding>().UpdateNode(position, false);
        Time.timeScale = lastTimeScale;
        creatingBuilding = false;
    }
}
