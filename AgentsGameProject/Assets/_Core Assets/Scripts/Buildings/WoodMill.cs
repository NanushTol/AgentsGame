using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodMill : MonoBehaviour
{
    //public float WorkersRadius = 1.5f;

    //public float WoodMillEfficiency = 2.3f;

    public int MaxWorkers = 6;

    //public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    //public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);


    [HideInInspector]
    public int CurrntlyWorking;

    float spherecastTimer = 0;

    int[] agentsWorking = new int[0];

    bool PositionValid;

    GenericBuilding genericBuilding;
    Environment environment;
    ResourcesData resourcesData;
    CostsUpkeepProductionData cupData;

    void Awake()
    {
        environment = GameObject.Find("Environment").GetComponent<Environment>();
        resourcesData = GameObject.Find("GameManager").GetComponent<ResourcesData>();
        cupData = GameObject.Find("GameManager").GetComponent<CostsUpkeepProductionData>();

        genericBuilding = GetComponent<GenericBuilding>();

        genericBuilding.WorkEfficiency = cupData.WoodMillWoodProduction;

        genericBuilding.MaxWorkers = MaxWorkers;
    }


    void Update()
    {
        if (genericBuilding.BuildingWorking && genericBuilding.Production > 0)
        {
            resourcesData.WoodProduction += genericBuilding.addedValue;
            resourcesData.WoodAmount += genericBuilding.addedValue;

            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }
}
