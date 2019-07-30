using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;
public class ResourcesDataController : MonoBehaviour
{
    public float UpddateSeconds = 1f;
    float elapsedTime;


    public GameEvent updateUiEvent;

    [Header("Game Date")]
    public FloatVariable Population;
    public FloatVariable YearsPlayed;

    [Header("Resources Date")]
    public List<FloatVariable> ResourcesProduction = new List<FloatVariable>();
    public List<FloatVariable> ResourcesAmounts = new List<FloatVariable>();


    [Header("Defualt Amounts")]
    public float DefualtGodForce;
    public float DefualtEnergy;
    public float DefualtFood;
    public float DefualtWater;
    public float DefualtStone;
    public float DefualtWood;
    public float DefualtMinerals;


    private void Awake()
    {
        // Reset Values do defualts
        ResourcesAmounts[GODFORCE].SetValue(DefualtGodForce);
        ResourcesAmounts[ENERGY].SetValue(DefualtEnergy);
        ResourcesAmounts[FOOD].SetValue(DefualtFood);
        ResourcesAmounts[WATER].SetValue(DefualtWater);
        ResourcesAmounts[STONE].SetValue(DefualtStone);
        ResourcesAmounts[WOOD].SetValue(DefualtWood);
        ResourcesAmounts[MINERALS].SetValue(DefualtMinerals);

        foreach (FloatVariable prod in ResourcesProduction)
        {
            prod.SetValue(0);
        }

        YearsPlayed.SetValue(0);
        Population.SetValue(0);

        UpdateResourcesValue();
    }


    void Update()
    {
        YearsPlayed.ApplyChange(Time.deltaTime);

        elapsedTime += Time.deltaTime;

        if(elapsedTime >= UpddateSeconds)
        {
            UpdateResourcesValue();

            elapsedTime = 0;
        }
    }

   

    void UpdateResourcesValue()
    {
        // add production to amount
        int i = 0;
        foreach(FloatVariable amount in ResourcesAmounts)
        {
            amount.ApplyChange(ResourcesProduction[i]);

            if (ResourcesAmounts[i].Value < 0) ResourcesAmounts[i].SetValue(0f);
            i++;
        }

        // update ui
        updateUiEvent.Raise();

        // reset production
        foreach(FloatVariable prod in ResourcesProduction)
        {
            prod.SetValue(0);
        }
    }

    public void UpdateResourceAmount(int index, float value)
    {
        ResourcesAmounts[index].ApplyChange(value);
    }

    public void UpdateResourceProduction(int index, float value)
    {
        ResourcesProduction[index].ApplyChange(value);
    }

    public float GetResourceAmount(int index)
    {
        return ResourcesAmounts[index].Value;
    }

    public float GetResourceProduction(int index)
    {
        return ResourcesProduction[index].Value;
    }

}
