using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneQuarry : MonoBehaviour
{
    public int MaxWorkers = 6;

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

        genericBuilding.WorkEfficiency = cupData.StoneQuarryProduction;

        genericBuilding.MaxWorkers = MaxWorkers;
    }

    void Update()
    {
        if(genericBuilding.BuildingWorking && genericBuilding.Production > 0)
        {
            resourcesData.StoneProduction += genericBuilding.addedValue;
            resourcesData.StoneAmount += genericBuilding.addedValue;

            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }    
}
