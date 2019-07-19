using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayCup : MonoBehaviour
{
    public enum DisplayCupType {StoneCost, StoneUpkeep, StoneProduction,
                WoodCost, WoodUpkeep, WoodProduction,
                MineralsCost, MineralsUpkeep, MineralsProduction,
                EnergyCost, EnergyUpkeep, EnergyProduction,
                GodForceCost, GodForceUpkeep, GodForceProduction,
                ResearchCost, ResearchUpkeep, ResearchProduction, 
                FoodCost, FoodUpkeep, FoodProduction,
                WaterCost, WaterUpkeep, WaterProduction
                }
    public DisplayCupType displayCupType;

    CostsUpkeepProductionData cupData;

    BuildingType buildingType;

    float displayValue;

    string parentName;

    string varName;

    void Awake()
    {
        //cupData = GameObject.Find("GameManager").GetComponent<CostsUpkeepProductionData>();
        //parentName = this.transform.parent.transform.parent.transform.parent.name;
        //parentName = parentName.Substring(0, parentName.Length - 6);

        buildingType = this.transform.parent.transform.parent.transform.parent.GetComponent<CreateBuilding>().BuildingType;

        switch (displayCupType)
        {
            #region //Stone CUP
            case DisplayCupType.StoneCost:
                displayValue = buildingType.StoneCost;
                break;

            case DisplayCupType.StoneUpkeep:
                displayValue = buildingType.StoneUpkeep;
                break;

            case DisplayCupType.StoneProduction:
                displayValue = buildingType.StoneProduction;
                break;
            #endregion

            #region //Wood CUP
            case DisplayCupType.WoodCost:
                displayValue = buildingType.WoodCost;
                break;

            case DisplayCupType.WoodUpkeep:
                displayValue = buildingType.WoodUpkeep;
                break;

            case DisplayCupType.WoodProduction:
                displayValue = buildingType.WoodProduction;
                break;
            #endregion

            #region //Minerals CUP
            case DisplayCupType.MineralsCost:
                displayValue = buildingType.MineralCost;
                break;

            case DisplayCupType.MineralsUpkeep:
                displayValue = buildingType.MineralUpkeep;
                break;

            case DisplayCupType.MineralsProduction:
                displayValue = buildingType.MineralProduction;
                break;
            #endregion

            #region //GodForce CUP
            case DisplayCupType.GodForceCost:
                displayValue = buildingType.GodForceCost;
                break;

            case DisplayCupType.GodForceUpkeep:
                displayValue = buildingType.GodForceUpkeep;
                break;

            case DisplayCupType.GodForceProduction:
                displayValue = buildingType.GodForceProduction;
                break;
            #endregion

            #region //Energy CUP
            case DisplayCupType.EnergyCost:
                displayValue = buildingType.EnergyCost;
                break;

            case DisplayCupType.EnergyUpkeep:
                displayValue = buildingType.EnergyUpkeep;
                break;

            case DisplayCupType.EnergyProduction:
                displayValue = buildingType.EnergyProduction;
                break;
            #endregion

            #region //Research CUP
            case DisplayCupType.ResearchCost:
                displayValue = buildingType.ResearchCost;
                break;

            case DisplayCupType.ResearchUpkeep:
                displayValue = buildingType.ResearchUpkeep;
                break;

            case DisplayCupType.ResearchProduction:
                displayValue = buildingType.ResearchProduction;
                break;
            #endregion

            #region //Food CUP
            case DisplayCupType.FoodCost:
                displayValue = buildingType.FoodCost;
                break;

            case DisplayCupType.FoodUpkeep:
                displayValue = buildingType.FoodUpkeep;
                break;

            case DisplayCupType.FoodProduction:
                displayValue = buildingType.FoodProduction;
                break;
            #endregion

            #region //water CUP
            case DisplayCupType.WaterCost:
                displayValue = buildingType.WaterCost;
                break;

            case DisplayCupType.WaterUpkeep:
                displayValue = buildingType.WaterUpkeep;
                break;

            case DisplayCupType.WaterProduction:
                displayValue = buildingType.WaterProduction;
                break;
            #endregion
        }
    }

        void Update()
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(displayValue).ToString();
    }
}
