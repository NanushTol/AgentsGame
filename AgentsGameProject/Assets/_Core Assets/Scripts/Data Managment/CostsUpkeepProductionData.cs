using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostsUpkeepProductionData : MonoBehaviour
{
    [Header("Wood Mill")]
    public float WoodMillStoneCost;
    public float WoodMillWoodCost;
    public float WoodMillEnergyUpkeep;
    public float WoodMillWaterUpkeep;
    public float WoodMillWoodProduction;

    [Header("Basic Farm")]
    public float BasicFarmStoneCost;
    public float BasicFarmWoodCost; 
    public float BasicFarmEnergyUpkeep;
    public float BasicFarmWaterUpkeep;
    public float BasicFarmMineralsUpkeep;
    public float BasicFarmProduction;

    public Dictionary<string, float> CupData;

    void Awake()
    {
        CupData = new Dictionary<string, float>()
        {
            {"WoodMillStoneCost", WoodMillStoneCost},
            {"WoodMillWoodCost", WoodMillWoodCost},
            {"WoodMillEnergyUpkeep", WoodMillEnergyUpkeep},
            {"WoodMillWaterUpkeep", WoodMillWaterUpkeep},
            {"WoodMillWoodProduction", WoodMillWoodProduction},

            {"BasicFarmStoneCost", BasicFarmStoneCost},
            {"BasicFarmWoodCost", BasicFarmWoodCost},
            {"BasicFarmEnergyUpkeep", BasicFarmEnergyUpkeep},
            {"BasicFarmWaterUpkeep", BasicFarmWaterUpkeep},
            {"BasicFarmMineralsUpkeep", BasicFarmMineralsUpkeep},
            {"BasicFarmProduction", BasicFarmProduction}
        };
    }
}
