using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneQuarry : MonoBehaviour
{
    public int MaxWorkers = 6;

    public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);


    [HideInInspector]
    public int CurrntlyWorking;

    float spherecastTimer = 0;

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
    }


    void Update()
    {
        spherecastTimer = spherecastTimer + Time.deltaTime;

        // If the building is working
        if (genericBuilding.BuildingWorking)
        {
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
                float addedValue = genericBuilding.Production;
                resourcesData.StoneProduction += addedValue;
                resourcesData.StoneAmount += resourcesData.StoneProduction;

                genericBuilding.Production -= addedValue;
            }

        }

        else if (genericBuilding.BuildingWorking == false)
        {
            genericBuilding.WorkersNeeded = false;
        }
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
