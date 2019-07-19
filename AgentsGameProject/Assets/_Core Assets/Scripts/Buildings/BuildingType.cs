using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildingType : ScriptableObject
{
    public GameObject BuildingPrefab;

    public string ResourceTag;

    [Header("Upkeep")]
    public float GodForceUpkeep;
    public float EnergyUpkeep;
    public float ResearchUpkeep;
    public float FoodUpkeep;
    public float WaterUpkeep;
    public float StoneUpkeep;
    public float WoodUpkeep;
    public float MineralUpkeep;

    [Header("Cost")]
    public float GodForceCost;
    public float ResearchCost;
    public float EnergyCost;
    public float FoodCost;
    public float WaterCost;
    public float StoneCost;
    public float WoodCost;
    public float MineralCost;

    [Header("Production")]
    public float GodForceProduction;
    public float EnergyProduction;
    public float ResearchProduction;
    public float FoodProduction;
    public float WaterProduction;
    public float StoneProduction;
    public float WoodProduction;
    public float MineralProduction;
    public float WasteProduction;
}
