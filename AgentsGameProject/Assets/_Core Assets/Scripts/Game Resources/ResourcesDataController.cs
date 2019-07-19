using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesDataController : MonoBehaviour
{
    public float Timer;
    float elapsedTime;

    public GameEvent updateUiEvent;

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
        ResourcesAmounts[StringToIndex("GodForce")].SetValue(DefualtGodForce);
        ResourcesAmounts[StringToIndex("Energy")].SetValue(DefualtEnergy);
        ResourcesAmounts[StringToIndex("Food")].SetValue(DefualtFood);
        ResourcesAmounts[StringToIndex("Water")].SetValue(DefualtWater);
        ResourcesAmounts[StringToIndex("Stone")].SetValue(DefualtStone);
        ResourcesAmounts[StringToIndex("Wood")].SetValue(DefualtWood);
        ResourcesAmounts[StringToIndex("Minerals")].SetValue(DefualtMinerals);

        foreach (FloatVariable prod in ResourcesProduction)
        {
            prod.SetValue(0);
        }

        UpdateResourcesValue();
    }


    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= Timer)
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

    public void UpdateResourceAmount(string _resourceName, float _value)
    {
        int _index = StringToIndex(_resourceName);
        ResourcesAmounts[_index].ApplyChange(_value);
    }

    public void UpdateResourceProduction(string _resourceName, float _value)
    {
        int _index = StringToIndex(_resourceName);
        ResourcesProduction[_index].ApplyChange(_value);
    }

    public float GetResourceAmount(string _resourceName)
    {
        float _value = 0;
        int _index = StringToIndex(_resourceName);

        _value = ResourcesAmounts[_index].Value;

        return _value;
    }

    public float GetResourceProduction(string _resourceName)
    {
        float _value = 0;
        int _index = StringToIndex(_resourceName);

        _value = ResourcesProduction[_index].Value;

        return _value;
    }

    private int StringToIndex(string _string)
    {
        if (_string.Contains("GodForce"))
            return 0;
        else if (_string.Contains("Energy"))
            return 1;
        else if (_string.Contains("Research"))
            return 2;
        else if (_string.Contains("Food"))
            return 3;
        else if (_string.Contains("Water"))
            return 4;
        else if (_string.Contains("Stone"))
            return 5;
        else if (_string.Contains("Wood"))
            return 6;
        else if (_string.Contains("Minerals"))
            return 7;

        return 999;
    }
}
