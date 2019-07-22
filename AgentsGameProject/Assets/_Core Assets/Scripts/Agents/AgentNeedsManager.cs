using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentNeedsManager : MonoBehaviour
{
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
    
    public enum Needs {NullNeed = 0, Hungry = 1, Tired = 2, Work = 3, Horny = 4}

    private void Awake()
    {
        NeedsValues = new float[5];
        NeedsValues[(int)Needs.NullNeed] = 0f;
    }

    public int FindMostUrgentNeed(Agent agent)
    {
        int index = (int)Needs.NullNeed;

        NeedsValues[(int)Needs.Hungry] = foodToHunger.Evaluate(AgentUtils.Remap(agent.Food, 0f, agent.AgentsSharedParameters.MaxFood, 0f, 1f));

        NeedsValues[(int)Needs.Tired] = energyToTiredness.Evaluate(AgentUtils.Remap(agent.Energy, 0f, agent.AgentsSharedParameters.MaxEnergy, 0f, 1f));

        NeedsValues[(int)Needs.Work] = energyToReadyness.Evaluate(AgentUtils.Remap(agent.Energy, 0f, agent.AgentsSharedParameters.MaxEnergy, 0f, 1f));

        NeedsValues[(int)Needs.Horny] = agent.ReproductiveMultiplier * ageToHorney.Evaluate(AgentUtils.Remap(agent.CurrentAge, 0f, agent.MaxAge, 0f, 1f));

        //find Biggest Value
        float mostUrgent = 0f;
        int i = 0;
        foreach (float need in NeedsValues)
        {
            if (need > mostUrgent)
                index = i;
            i++;
        }

        return index;
    }
}
