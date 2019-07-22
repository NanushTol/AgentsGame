using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    private const int NULLNEED = 0;
    private const int HUNGRY = 1;
    private const int TIRED = 2;
    private const int WORK = 3;
    private const int HORNY = 4;


    #region // State Functions

    public void Enter()
    {

    }

    public void ExecuteState()
    {
        if (Owner.MostUrgentNeedByIndex == WORK)
            Work(Owner.CurrentWorkplace);
        else
            Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
    }

    public void Exit()
    {
        ExitBuilding();
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion

    #region // Extention Functions

    void Work(GenericBuilding currentWorkplace)
    {
        float workPlaceEfficiency = currentWorkplace.GetComponent<GenericBuilding>().WorkEfficiency;
        float workPlaceProduction = currentWorkplace.GetComponent<GenericBuilding>().Production;

        //create Production
        workPlaceProduction += workPlaceEfficiency * Owner.WorkingSpeed * Time.deltaTime;
        currentWorkplace.GetComponent<GenericBuilding>().Production = workPlaceProduction;

        //reduce Energy & Food
        Owner.Food -= (Owner.FoodConsumption * Time.deltaTime * Owner.AgentsSharedParameters.ConsumptionScale);
        Owner.Energy -= (Owner.EnergyConsumption * Time.deltaTime * Owner.AgentsSharedParameters.ConsumptionScale);
    }

    void ExitBuilding()
    {
        Owner.transform.position = Owner.BuildingEnterPosition;
        Owner.CurrentWorkplace.AgentsWorking -= 1;
        Owner.SetBuildingState(false);
    }

    #endregion
}
