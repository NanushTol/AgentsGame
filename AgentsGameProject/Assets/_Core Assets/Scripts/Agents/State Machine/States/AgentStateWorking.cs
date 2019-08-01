using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateWorking : IAgentState
{
    public Agent Owner;

    public AgentStateWorking(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Working"; }

    }
    public Color StateColor
    {
        get { return Color.green; }
    }


    #region // State Functions

    public void Enter()
    {
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.Working;
    }

    public void ExecuteState()
    {
        if (StatesUtils.ValidateState(Owner, WORK) && Owner.CurrentWorkplace.BuildingActive)
            Work(Owner.CurrentWorkplace);
        else
            Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
    }

    public void Exit()
    {
        StatesUtils.ExitBuilding(Owner, WORK);
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion


    #region // Extention Functions

    void Work(GenericBuilding currentWorkplace)
    {
        //create Production
        float workPlaceEfficiency = currentWorkplace.ProductionRate;
        currentWorkplace.Production += workPlaceEfficiency * Owner.WorkingSpeed * Time.deltaTime;

        //reduce Energy & Food
        float consunmptionScale = Owner.AgentsSharedParameters.ConsumptionScale;
        Owner.Food -= (Owner.FoodConsumption * Time.deltaTime * consunmptionScale);
        Owner.Energy -= (Owner.EnergyConsumption * Time.deltaTime * consunmptionScale);
    }

    #endregion
}
