using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFarm : MonoBehaviour
{
    public GameObject FoodPrefab;

    public FloatVariable FoodProduction;

    public float GrowingRadius = 7f;

    public int MaxWorkers = 6;



    bool feedingFruit = false;

    GameObject foodSource = null;

    bool PositionValid;

    GenericBuilding genericBuilding;
 

    void Awake()
    {
        genericBuilding = GetComponent<GenericBuilding>();

        genericBuilding.WorkEfficiency = genericBuilding.buildingType.FoodProduction;

        genericBuilding.MaxWorkers = MaxWorkers;
    }

    void Update()
    {
        // Checks if the building is working and if there is production to use
        if (genericBuilding.Production > 0 && genericBuilding.BuildingWorking)
        {
            // Update food resource production
            genericBuilding.resourcesDataController.UpdateResourceProduction("Food", genericBuilding.addedValue);

            // If no fruit available create fruit
            if (foodSource == null || feedingFruit == false)
            {

                Vector3 foodPosition = new Vector3(UnityEngine.Random.Range(-GrowingRadius, GrowingRadius),
                    UnityEngine.Random.Range(-GrowingRadius, GrowingRadius), 0f);
                Quaternion _rotation = new Quaternion(0, 0, 0, 0);

                for (int i = 0; i < 20; i++)
                {
                    PositionValid = Physics2D.Raycast(foodPosition + transform.position, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                    if (PositionValid)
                    {
                        i = 21;
                    }
                }

                // instantiate Food as Seed
                if (PositionValid)
                {
                    foodSource = Instantiate(FoodPrefab, foodPosition + transform.position, _rotation);

                    foodSource.GetComponent<Food>().FoodValue += genericBuilding.addedValue;

                    PositionValid = false;
                }
            }

            // If there is a fruit feed fruit up to 20(max fruit value)
            if (foodSource != null) 
            {
                if (foodSource.GetComponent<Food>().FoodValue < 20) // feed fruit
                {
                    foodSource.GetComponent<Food>().FoodValue += genericBuilding.addedValue;


                    feedingFruit = true;
                }

                // Finish feeding and relese fruit
                if (foodSource.GetComponent<Food>().FoodValue >= 20) 
                {
                    foodSource.tag = "Food";
                    foodSource.layer = 8;
                    feedingFruit = false;
                    foodSource = null;
                }
            }

            // Reset pruduction
            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }


}
