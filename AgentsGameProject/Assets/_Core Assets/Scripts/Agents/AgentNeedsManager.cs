using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentNeedsManager : MonoBehaviour
{
    [Header("Needs Values")]
    public float Hunger;
    public float Tired;
    public float Work;
    public float Horny;

    [Header("Hunger Graphs")]
    public AnimationCurve foodToHunger;

    [Header("Work Graphs")]
    public AnimationCurve energyToReadyness;

    [Header("Sleep Graphs")]
    public AnimationCurve energyToTiredness;

    [Header("Horny Graphs")]
    public AnimationCurve ageToHorney;

    [HideInInspector]
    public float[] NeedsValues;

    public bool WorkNeedOverride;
    public bool FoodNeedOverride;

    private void Awake()
    {
        NeedsValues = new float[5];
        NeedsValues[NULLNEED] = 0f;
    }

    void LateUpdate()
    {
        Hunger = NeedsValues[HUNGRY];
        Tired = NeedsValues[TIRED];
        Work = NeedsValues[WORK];
        Horny = NeedsValues[HORNY];
    }

    public int FindMostUrgentNeed(Agent agent)
    {
        int index = NULLNEED;


        // Remap Values
        float rmpFood = AgentUtils.Remap(agent.Food, 0f, agent.AgentsSharedParameters.MaxFood, 0f, 1f);
        float rmpEnergy = AgentUtils.Remap(agent.Energy, 0f, agent.AgentsSharedParameters.MaxEnergy, 0f, 1f);
        float rmpAge = AgentUtils.Remap(agent.CurrentAge, 0f, agent.MaxAge, 0f, 1f);

        //FoodNeedOverride = ValidateOverride(foodToHunger, rmpFood);

        // Evaluate Need Curves
        NeedsValues[HUNGRY] = foodToHunger.Evaluate(rmpFood) * OverrideNeed(FoodNeedOverride);
        NeedsValues[TIRED] = energyToTiredness.Evaluate(rmpEnergy);
        NeedsValues[WORK] = energyToReadyness.Evaluate(rmpEnergy) * OverrideNeed(WorkNeedOverride);
        NeedsValues[HORNY] = agent.ReproductiveMultiplier * ageToHorney.Evaluate(rmpAge);


        //find Biggest Value
        float mostUrgent = 0f;
        int i = 0;
        foreach (float need in NeedsValues)
        {
            if (need > mostUrgent)
            {
                mostUrgent = need;
                index = i;
            }
               
            i++;
        }

        return index;
    }

    public float OverrideNeed(bool overrideNeed)
    {
        return overrideNeed ? 0 : 1;
    }

    public bool ValidateOverride(AnimationCurve needGraph, float rampedValue)
    {
        float needVal = needGraph.Evaluate(rampedValue);

        if (needVal >= 0.9f) return false;
        else return true;
    }
}
