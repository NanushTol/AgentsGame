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

    float displayValue;

    string parentName;

    string varName;

    void Awake()
    {
        cupData = GameObject.Find("GameManager").GetComponent<CostsUpkeepProductionData>();
        parentName = this.transform.parent.transform.parent.transform.parent.name;
        parentName = parentName.Substring(0, parentName.Length - 6);

        switch (displayCupType)
        {
            #region //Stone CUP
            case DisplayCupType.StoneCost:
                varName = parentName + "StoneCost";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.StoneUpkeep:
                varName = parentName + "StoneUpkeep";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.StoneProduction:
                varName = parentName + "StoneProduction";
                displayValue = cupData.CupData[varName];
                break;
            #endregion

            #region //Wood CUP
            case DisplayCupType.WoodCost:
                varName = parentName + "WoodCost";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.WoodUpkeep:
                varName = parentName + "WoodUpkeep";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.WoodProduction:
                varName = parentName + "WoodProduction";
                displayValue = cupData.CupData[varName];
                break;
            #endregion

            #region //Minerals CUP
            case DisplayCupType.MineralsCost:
                varName = parentName + "MineralsCost";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.MineralsUpkeep:
                varName = parentName + "MineralsUpkeep";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.MineralsProduction:
                varName = parentName + "MineralsProduction";
                displayValue = cupData.CupData[varName];
                break;
            #endregion

            #region //GodForce CUP
            case DisplayCupType.GodForceCost:
                varName = parentName + "GodForceCost";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.GodForceUpkeep:
                varName = parentName + "GodForceUpkeep";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.GodForceProduction:
                varName = parentName + "GodForceProduction";
                displayValue = cupData.CupData[varName];
                break;
            #endregion

            #region //Energy CUP
            case DisplayCupType.EnergyCost:
                varName = parentName + "EnergyCost";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.EnergyUpkeep:
                varName = parentName + "EnergyUpkeep";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.EnergyProduction:
                varName = parentName + "EnergyProduction";
                displayValue = cupData.CupData[varName];
                break;
            #endregion

            #region //Research CUP
            case DisplayCupType.ResearchCost:
                varName = parentName + "ResearchCost";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.ResearchUpkeep:
                varName = parentName + "ResearchUpkeep";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.ResearchProduction:
                varName = parentName + "ResearchProduction";
                displayValue = cupData.CupData[varName];
                break;
            #endregion

            #region //Food CUP
            case DisplayCupType.FoodCost:
                varName = parentName + "FoodCost";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.FoodUpkeep:
                varName = parentName + "FoodUpkeep";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.FoodProduction:
                varName = parentName + "FoodProduction";
                displayValue = cupData.CupData[varName];
                break;
            #endregion

            #region //water CUP
            case DisplayCupType.WaterCost:
                varName = parentName + "WaterCost";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.WaterUpkeep:
                varName = parentName + "WaterUpkeep";
                displayValue = cupData.CupData[varName];
                break;

            case DisplayCupType.WaterProduction:
                varName = parentName + "WaterProduction";
                displayValue = cupData.CupData[varName];
                break;
            #endregion
        }
    }

        void Update()
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(displayValue).ToString();
    }
}
