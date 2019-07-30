﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class PowerPlant : MonoBehaviour
{
    public int MaxWorkers = 6;
    
    GenericBuilding genericBuilding;
   
    void Awake()
    {
        genericBuilding = GetComponent<GenericBuilding>();

        genericBuilding.ProductionRate = genericBuilding.BuildingType.EnergyProduction;

        genericBuilding.MaxWorkers = MaxWorkers;
    }

    void Update()
    {
        // Checks if the building is working and if there is production to use
        if (genericBuilding.BuildingActive && genericBuilding.Production > 0)
        {
            // Update resource production
            genericBuilding.resourcesDataController.UpdateResourceProduction(ENERGY, genericBuilding.addedValue);

            // Reset pruduction
            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }    
}
