using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFarm : MonoBehaviour
{
    public GameObject FoodPrefab;

    public float GrowingRadius = 7f;
    //public float WorkersRadius = 1.5f;

    public int MaxWorkers = 6;
    //public int CurrntlyWorking;

    //public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    //public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    bool feedingFruit = false;

    GameObject foodSource = null;

    Environment environment;

    ResourcesData resourcesData;


    float spherecastTimer = 0;

    //int[] agentsWorking = new int[0];

    bool PositionValid;

    GenericBuilding genericBuilding;
    CostsUpkeepProductionData cupData;

    //public float basicFarmEfficiency = 2.3f;

    void Awake()
    {
        environment = GameObject.Find("Environment").GetComponent<Environment>();
        resourcesData = GameObject.Find("GameManager").GetComponent<ResourcesData>();
        cupData = GameObject.Find("GameManager").GetComponent<CostsUpkeepProductionData>();

        genericBuilding = GetComponent<GenericBuilding>();

        genericBuilding.WorkEfficiency = cupData.BasicFarmFoodProduction;

        genericBuilding.MaxWorkers = MaxWorkers;
    }

    void Update()
    {
        if (genericBuilding.Production > 0 && genericBuilding.BuildingWorking)
        {
            resourcesData.FoodProduction += genericBuilding.addedValue;
            resourcesData.FoodAmount += genericBuilding.addedValue;

            // if no fruit available create fruit
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

            //If there is a fruit feed fruit up to 20(max fruit value)
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

            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }


}
