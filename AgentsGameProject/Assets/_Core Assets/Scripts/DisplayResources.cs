using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayResources : MonoBehaviour
{
    public enum ProduceType { None, GodForceAmount, GodForceProduction, GodForceProductionSign,
                EnergyProduction, EnergyProductionSign, ReaserchProduction,
                FoodAmount, FoodProduction, FoodProductionSign, WaterProduction, WaterProductionSign}
    public ProduceType produceType;

    public enum ResourceType { None, StoneAmount, StoneProduction, WoodAmount, WoodProduction, MineralsAmount, MineralsProduction}
    public ResourceType resourceType;

    public bool IsItADisplaySign;

    bool displayValueSign; 

    float displayValue;

    ResourcesData resourcesManager;

    int index;

    void Awake()
    {
        resourcesManager = GameObject.Find("GameManager").GetComponent<ResourcesData>();

        switch(produceType)
        {
            case ProduceType.None:
                break;

            case ProduceType.GodForceAmount:
                displayValue = resourcesManager.GodForceAmount;
                index = 0;
                break;

            case ProduceType.GodForceProduction:
                displayValue = resourcesManager.GodForceProduction;
                index = 1;
                break;

            case ProduceType.EnergyProduction:
                displayValue = resourcesManager.EnergyProduction;
                index = 2;
                break;

            case ProduceType.ReaserchProduction:
                displayValue = resourcesManager.ReaserchProduction;
                index = 3;
                break;

            case ProduceType.FoodAmount:
                displayValue = resourcesManager.FoodAmount;
                index = 4;
                break;

            case ProduceType.FoodProduction:
                displayValue = resourcesManager.FoodProduction;
                index = 5;
                break;

            case ProduceType.WaterProduction:
                displayValue = resourcesManager.WaterProduction;
                index = 6;
                break;
        }



        switch (resourceType)
        {
            case ResourceType.None:
                break;

            case ResourceType.StoneAmount:
                displayValue = resourcesManager.StoneAmount;
                index = 7;
                break;

            case ResourceType.StoneProduction:
                displayValue = resourcesManager.StoneProduction;
                index = 8;
                break;

            case ResourceType.WoodAmount:
                displayValue = resourcesManager.WoodAmount;
                index = 9;
                break;

            case ResourceType.WoodProduction:
                displayValue = resourcesManager.WoodProduction;
                index = 10;
                break;

            case ResourceType.MineralsAmount:
                displayValue = resourcesManager.MineralsAmount;
                index = 11;
                break;

            case ResourceType.MineralsProduction:
                displayValue = resourcesManager.MineralsProduction;
                index = 12;
                break;
        }
    }


    void Update()
    {
        if (IsItADisplaySign)
        {
            SignType();
        }
        else if (IsItADisplaySign == false)
        {
            if (index == 0 || index == 2 || index == 4 || index == 7 || index == 9 || index == 11)
            {
                this.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(resourcesManager.ResourceByIndex[index]).ToString("0");
            }
            else
            {
                this.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(resourcesManager.ResourceByIndex[index]).ToString("0.00");
            }
        }
    }

    void SignType()
    {
        switch (produceType)
        {
            case ProduceType.GodForceProductionSign:
                if (resourcesManager.GodForceProduction < 0)
                { displayValueSign = false; }
                else { displayValueSign = true; }
                break;

            case ProduceType.EnergyProductionSign:
                if (resourcesManager.EnergyProduction < 0)
                { displayValueSign = false; }
                else { displayValueSign = true; }
                break;

            case ProduceType.FoodProductionSign:
                if (resourcesManager.FoodProduction < 0)
                { displayValueSign = false; }
                else { displayValueSign = true; }
                break;

            case ProduceType.WaterProductionSign:
                if (resourcesManager.WaterProduction < 0)
                { displayValueSign = false; }
                else { displayValueSign = true; }
                break;
        }


        if (displayValueSign == false)
        { this.gameObject.GetComponent<TextMeshProUGUI>().text = "-"; }

        else if (displayValueSign)
        { this.gameObject.GetComponent<TextMeshProUGUI>().text = "+"; }
    }
}
