using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using static Constants;

public static class StatesUtils
{ 
   public static bool ValidateState(Agent agent, int stateNeedIndex)
    {
        bool valid = false;

        if (agent.MostUrgentNeedByIndex == stateNeedIndex)
            valid = true;

        return valid;
    }

    public static void MoveTo(Agent owner, GameObject target)
    {
        owner.aiDestinationSetter.target = target.transform;

        float velocity = owner.GetComponent<Rigidbody2D>().velocity.magnitude;

        owner.Energy -= (owner.EnergyConsumption * owner.Size * Time.deltaTime * owner.AgentsSharedParameters.ConsumptionScale);
        owner.Food -= (owner.FoodConsumption * owner.Size * Time.deltaTime * owner.AgentsSharedParameters.ConsumptionScale);
    }

    public static void SetBuildingState(Agent owner, bool building)
    {
        owner.InBuilding = !building;

        owner.transform.GetChild(0).gameObject.SetActive(building);
        owner.transform.GetChild(1).gameObject.SetActive(building);
        owner.transform.GetChild(2).gameObject.SetActive(building);
        owner.transform.GetChild(3).gameObject.SetActive(building);

        owner.GetComponent<AIPath>().enabled = building;
        owner.GetComponent<AIDestinationSetter>().enabled = building;
        owner.GetComponent<SimpleSmoothModifier>().enabled = building;
        owner.GetComponent<Collider2D>().enabled = building;
        owner.GetComponent<Rigidbody2D>().simulated = building;
    }

    public static void EnterBuilding(Agent owner, int need)
    {
        owner.BuildingEnterPosition = owner.transform.position;
        switch (need)
        {
            case WORK:
                owner.CurrentWorkplace.AgentsWorking += 1;
                break;
            case HUNGRY:
                owner.ChosenFoodPlace.FeedingAgents += 1;
                break;
            case TIRED:
                owner.CurrentSleepPlace.SleepingAgents += 1;
                break;
        }
        SetBuildingState(owner, false);
    }

    public static void ExitBuilding(Agent owner, int need)
    {
        owner.transform.position = owner.BuildingEnterPosition;
        switch (need)
        {
            case WORK:
                owner.CurrentWorkplace.AgentsWorking -= 1;
                break;
            case HUNGRY:
                owner.ChosenFoodPlace.FeedingAgents -= 1;
                break;
            case TIRED:
                owner.CurrentSleepPlace.SleepingAgents -= 1;
                break;
        }
        SetBuildingState(owner, true);
    }
}
