using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateSearchingSleepingPlace : IAgentState
{
    public Agent Owner;

    public AgentStateSearchingSleepingPlace(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Searching Sleep Place"; }

    }
    public Color StateColor
    {
        get { return Color.blue; }
    }

    public string LayerMaskString = "SleepPlace";
    public string Tag = "Sleep";

    #region // State Functions

    public void Enter()
    {
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.SearchingSleep;
    }

    public void ExecuteState()
    {
        AddNewSleepPlacesToMemory();

        Owner.CurrentSleepPlace = FindClosestSleepPlace();

        Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.MovingToSleep]);
    }

    public void Exit()
    {
        
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion

    #region // Extention Functions

    void AddNewSleepPlacesToMemory()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Owner.transform.position, Owner.AgentsSharedParameters.SearchRadius, LayerMask.GetMask(LayerMaskString));

        foreach (Collider2D collider in colliders)
        {
                if (Owner.AgentMemory.SleepingPlaces.Contains(collider.GetComponent<SleepPlace>()) == false)
                {
                    Owner.AgentMemory.AddItemToList(collider.GetComponent<SleepPlace>(), Owner.AgentMemory.SleepingPlaces);
                }
        }
    }

    SleepPlace FindClosestSleepPlace()
    {
        float closestObjectDistance = 1000f;
        SleepPlace closestObject = null;

        foreach (SleepPlace item in Owner.AgentMemory.SleepingPlaces)
        {
            float distanceToObject = Vector3.Distance(item.transform.position, Owner.transform.position);

            //check if distance is smaller the the closest one yet
            if (distanceToObject < closestObjectDistance)
            {
                if (item.gameObject.name != Owner.name)
                {
                    closestObject = item;
                    closestObjectDistance = distanceToObject;
                }
            }
        }
        return closestObject;
    }
    #endregion
}
