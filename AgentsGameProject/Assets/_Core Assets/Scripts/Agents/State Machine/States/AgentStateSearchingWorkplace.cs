using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateSearchingWorkplace : IAgentState
{
    public Agent Owner;

    public AgentStateSearchingWorkplace(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Searching Workplace"; }

    }
    public Color StateColor
    {
        get { return Color.gray; }
    }

    public string LayerMaskString = "Building";
    public string Tag = "Work";

    #region // State Functions

    public void Enter()
    {
        
    }

    public void ExecuteState()
    {
        AddNewWorkplacesToMemory();
        Owner.DecisionMaker.SortDictionaryByValues(Owner.AgentMemory.Workplaces);

        Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.MovingToWork]);
    }

    public void Exit()
    {

    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion

    #region // Extention Functions

    void AddNewWorkplacesToMemory()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Owner.transform.position, Owner.AgentsSharedParameters.SearchRadius, LayerMask.GetMask(LayerMaskString));

        foreach (Collider2D collider in colliders)
        {
            if (Owner.AgentMemory.Workplaces.ContainsKey(collider.GetComponent<GenericBuilding>()) == false)
            {
                Owner.AgentMemory.AddItemToDictionary(collider.GetComponent<GenericBuilding>(), Owner.AgentMemory.Workplaces);
            }
        }
    }

    #endregion
}
