using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.SearchingWork;
    }

    public void ExecuteState()
    {
        AddNewWorkplacesToMemory();

        Owner.DecisionMaker.ScoreItemsInDictionary(Owner.AgentMemory.Workplaces);

        Owner.AgentMemory.Workplaces = Owner.DecisionMaker.SortDictionaryByValues(Owner.AgentMemory.Workplaces);


        Owner.CurrentWorkplace = null;

        for (int i = 0; i < Owner.AgentMemory.Workplaces.Count; i++)
        {
            //if first entry Workplace - needs workers & building is working asign CurrentWorkplace
            if (Owner.AgentMemory.Workplaces.ElementAt(i).Key.WorkersNeeded && Owner.AgentMemory.Workplaces.ElementAt(i).Key.BuildingActive)
            {
                Owner.CurrentWorkplace = Owner.AgentMemory.Workplaces.ElementAt(i).Key;
                Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.MovingToWork]);
            }
        }

        if(Owner.CurrentWorkplace == null)
        {
            Owner.NeedsManager.WorkNeedOverride = true;
            Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
        }
           
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
