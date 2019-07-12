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

    [Header("Stone Quarry")]
    public float StoneQuarryStoneCost;
    public float StoneQuarryWoodCost;
    public float StoneQuarryEnergyUpkeep;
    public float StoneQuarryWaterUpkeep;
    public float StoneQuarryWoodProduction;

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

            {"StoneQuarryStoneCost", StoneQuarryStoneCost},
            {"StoneQuarryWoodCost", StoneQuarryWoodCost},
            {"StoneQuarryEnergyUpkeep", StoneQuarryEnergyUpkeep},
            {"StoneQuarryWaterUpkeep", StoneQuarryWaterUpkeep},
            {"StoneQuarryWoodProduction", StoneQuarryWoodProduction},

            {"BasicFarmStoneCost", BasicFarmStoneCost},
            {"BasicFarmWoodCost", BasicFarmWoodCost},
            {"BasicFarmEnergyUpkeep", BasicFarmEnergyUpkeep},
            {"BasicFarmWaterUpkeep", BasicFarmWaterUpkeep},
            {"BasicFarmMineralsUpkeep", BasicFarmMineralsUpkeep},
            {"BasicFarmProduction", BasicFarmProduction}
        };
    }
}
