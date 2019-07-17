using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesData : MonoBehaviour
{

    #region // Produce Variables

    public float GodForceAmount;
    public float GodForceProduction;

    public float EnergyProduction;

    public float ReaserchProduction;

    public float FoodAmount;
    public float FoodProduction;

    public float WaterProduction;

    #endregion

    #region // Resources Variables

    public float StoneAmount;
    public float StoneProduction;

    public float WoodAmount;
    public float WoodProduction;

    public float MineralsAmount;
    public float MineralsProduction;

    #endregion

    [HideInInspector]
    public float[] ResourceByIndex;


    void Awake()
    {
        ResourceByIndex = new float[13];  
    }
    void Update()
    {
        WoodProduction = 0;
        StoneProduction = 0;
        FoodProduction = 0;
        WaterProduction = 0;
        EnergyProduction = 0;
        //MineralsProduction = 100f;
    }

    void LateUpdate()
    {
        //WoodProduction *= 1 / Time.deltaTime;

        ResourceByIndex[0] = GodForceAmount;
        ResourceByIndex[1] = GodForceProduction;
        ResourceByIndex[2] = EnergyProduction;
        ResourceByIndex[3] = ReaserchProduction;
        ResourceByIndex[4] = FoodAmount;
        ResourceByIndex[5] = FoodProduction;
        ResourceByIndex[6] = WaterProduction;
        ResourceByIndex[7] = StoneAmount;
        ResourceByIndex[8] = StoneProduction;
        ResourceByIndex[9] = WoodAmount;
        ResourceByIndex[10] = WoodProduction;
        ResourceByIndex[11] = MineralsAmount;
        ResourceByIndex[12] = MineralsProduction;
    }
}
