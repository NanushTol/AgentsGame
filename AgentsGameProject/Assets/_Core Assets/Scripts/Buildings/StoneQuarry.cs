using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneQuarry : MonoBehaviour
{
    public int MaxWorkers = 6;

    GenericBuilding genericBuilding;

    void Awake()
    {
        genericBuilding = GetComponent<GenericBuilding>();

        genericBuilding.WorkEfficiency = genericBuilding.buildingType.StoneProduction;

        genericBuilding.MaxWorkers = MaxWorkers;
    }

    void Update()
    {
        // Checks if the building is working and if there is production to use
        if (genericBuilding.BuildingWorking && genericBuilding.Production > 0)
        {
            // Update resource production
            genericBuilding.resourcesDataController.UpdateResourceProduction("Stone", genericBuilding.addedValue);
            
            // Reset pruduction
            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }    
}
