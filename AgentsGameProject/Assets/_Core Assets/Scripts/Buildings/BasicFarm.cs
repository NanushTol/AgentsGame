using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class BasicFarm : MonoBehaviour
{
    public FloatVariable FoodProduction;

    public int MaxWorkers = 6;

    Food food;

    GenericBuilding genericBuilding;
 

    void Awake()
    {
        genericBuilding = GetComponent<GenericBuilding>();

        food = GetComponent<Food>();

        genericBuilding.ProductionRate = genericBuilding.BuildingType.FoodProduction;

        genericBuilding.MaxWorkers = MaxWorkers;
    }
    
    void Update()
    {
        // Checks if the building is working and if there is production to use
        if (genericBuilding.Production > 0 && genericBuilding.BuildingActive)
        {
            if(food.FoodValue < food.MaxFood)
            {
                food.FoodValue += genericBuilding.addedValue;

                // Update food resource production
                genericBuilding.resourcesDataController.UpdateResourceProduction(FOOD, genericBuilding.addedValue);
            }
            
            // Reset production
            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }


}
