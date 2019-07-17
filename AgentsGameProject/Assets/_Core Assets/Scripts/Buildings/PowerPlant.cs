using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPlant : MonoBehaviour
{
    public int MaxWorkers = 6;

    float BaseProduction;

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

        genericBuilding.WorkEfficiency = cupData.PowerPlantProduction;

        genericBuilding.MaxWorkers = MaxWorkers;

        BaseProduction = cupData.PowerPlantBaseProduction;
    }

    void Update()
    {
        resourcesData.EnergyProduction += genericBuilding.addedValue + BaseProduction;

        if (genericBuilding.BuildingWorking && genericBuilding.Production > 0)
        {
            

            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }    
}
