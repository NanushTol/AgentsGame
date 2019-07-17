using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWaterPump : MonoBehaviour
{

    public int MaxWorkers = 6;

    float BaseProduction;

    //public float WorkersRadius = 1.5f;

    //public float WoodMillEfficiency = 2.3f;

    //public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    //public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    //[HideInInspector]
    //public int CurrntlyWorking;

    //float spherecastTimer = 0;

    //int[] agentsWorking = new int[0];

    //bool PositionValid;

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

        genericBuilding.WorkEfficiency = cupData.BasicWaterPumpWaterProduction;

        genericBuilding.MaxWorkers = MaxWorkers;

        BaseProduction = cupData.BasicWaterPumpWaterBaseProduction;
    }


    void Update()
    {
        resourcesData.WaterProduction += genericBuilding.addedValue + BaseProduction;

        if (genericBuilding.BuildingWorking && genericBuilding.Production > 0)
        {
            

            genericBuilding.Production -= genericBuilding.addedValue;
        }
    }
}
