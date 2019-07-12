using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFarm : MonoBehaviour
{
    public GameObject FoodPrefab;

    public float GrowingRadius = 7f;
    public float WorkersRadius = 1.5f;

    public int MaxWorkers = 6;
    //public int CurrntlyWorking;

    public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    bool feedingFruit = false;

    GameObject foodSource = null;

    Environment environment;

    float spherecastTimer = 0;

    //int[] agentsWorking = new int[0];

    bool PositionValid;

    GenericBuilding genericBuilding;
    CostsUpkeepProductionData cupData;

    //public float basicFarmEfficiency = 2.3f;

    void Awake()
    {
        environment = GameObject.Find("Environment").GetComponent<Environment>();
        cupData = GameObject.Find("GameManager").GetComponent<CostsUpkeepProductionData>();
        genericBuilding = GetComponent<GenericBuilding>();
        genericBuilding.WorkEfficiency = cupData.BasicFarmProduction;
    }

    void Update()
    {
        //spherecastTimer = spherecastTimer + Time.deltaTime;

        if (genericBuilding.BuildingWorking)
        {
            #region // Sphere Cast Checks
            /*
            if (spherecastTimer >= 0.5)
            {
                Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(transform.position, WorkersRadius, LayerMask.GetMask("Agent"));



                CurrntlyWorking = 0;

                for (int a = 0; a < _objectColliders.Length; a++)
                {
                    if (_objectColliders[a].GetComponent<Agent>().mostUrgentNeedIndex == 2) // most urgent need = Work
                    {
                        CurrntlyWorking += 1;
                    }
                }

                agentsWorking = new int[CurrntlyWorking];

                if (agentsWorking.Length <= 6)
                {

                    genericBuilding.WorkersNeeded = true;

                    if (agentsWorking.Length == 6)
                    {
                        genericBuilding.WorkersNeeded = false;
                    }

                }
                if (agentsWorking.Length > 6)
                {
                    agentsWorking = new int[6];
                    genericBuilding.WorkersNeeded = false;
                }


                spherecastTimer = 0;
            }
            */
            #endregion

            

            if (genericBuilding.AgentsWorking <= MaxWorkers)
            {

                genericBuilding.WorkersNeeded = true;

                if (genericBuilding.AgentsWorking == MaxWorkers)
                {
                    genericBuilding.WorkersNeeded = false;
                }

            }
            if (genericBuilding.AgentsWorking > MaxWorkers)
            {
                genericBuilding.AgentsWorking = MaxWorkers;
                genericBuilding.WorkersNeeded = false;
            }

            UpdateVacancyBar(genericBuilding.AgentsWorking);

            if (genericBuilding.Production > 0)
            {
                if (foodSource == null || feedingFruit == false) // if no fruit available create fruit
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

                    if (PositionValid)
                    {
                        foodSource = Instantiate(FoodPrefab, foodPosition + transform.position, _rotation);

                        //genericBuilding.Production = genericBuilding.Production - (genericBuilding.WorkEfficiency * Time.deltaTime * agentsWorking.Length);
                        //foodSource.GetComponent<Food>().FoodValue += genericBuilding.WorkEfficiency * Time.deltaTime * agentsWorking.Length;

                        float foodAddedValue = genericBuilding.Production;

                        foodSource.GetComponent<Food>().FoodValue += foodAddedValue;

                        genericBuilding.Production -= foodAddedValue;

                        PositionValid = false;
                    }
                }

                if (foodSource != null) //If there is a fruit feed fruit up to 20(max fruit value)
                {
                    if (foodSource.GetComponent<Food>().FoodValue < 20) // feed fruit
                    {
                        //foodSource.GetComponent<Food>().FoodValue += genericBuilding.WorkEfficiency * Time.deltaTime * agentsWorking.Length * environment.HeatEfficiency;

                        //genericBuilding.Production -= (genericBuilding.WorkEfficiency * Time.deltaTime * agentsWorking.Length * environment.HeatEfficiency);

                        //environment.WorkTempIn += environment.WorkTempCost * agentsWorking.Length;

                        float foodAddedValue = genericBuilding.Production;

                        foodSource.GetComponent<Food>().FoodValue += foodAddedValue;

                        genericBuilding.Production -= foodAddedValue;

                        feedingFruit = true;
                    }



                    if (foodSource.GetComponent<Food>().FoodValue >= 20) // Finish feeding and relese fruit
                    {
                        foodSource.tag = "Food";
                        foodSource.layer = 8;
                        feedingFruit = false;
                        foodSource = null;
                    }
                }
            }

        }

        else if (genericBuilding.BuildingWorking == false)
        {
            genericBuilding.WorkersNeeded = false;
        }

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GrowingRadius);

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, WorkersRadius);

    }


    void UpdateVacancyBar(int _agentsWorking)
    {
        Transform vacancyBar = transform.GetChild(0);

        for (int j = 0; j < MaxWorkers; j++)
        {
            vacancyBar.transform.GetChild(j).gameObject.GetComponent<SpriteRenderer>().color = NotWorkingColor;
        }

        for (int i = 0; i < _agentsWorking; i++)
        {
            vacancyBar.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = WorkingColor;
        }

    }
}
