using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateEating : IAgentState
{
    public Agent Owner;

    public AgentStateEating(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Eating"; }

    }
    public Color StateColor
    {
        get { return Color.yellow; }
    }


    #region // State Functions

    public void Enter()
    {
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.Eating;
    }

    public void ExecuteState()
    {
        if (Owner.Food < Owner.AgentsSharedParameters.FoodFullThreshold && Owner.ChosenFoodPlace.FoodValue > 2f)
            Eat(Owner.ChosenFoodPlace);

        else if(Owner.ChosenFoodPlace.FoodValue < 2f)
            Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.SearchingFood]);

        else
            Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
    }

    public void Exit()
    {
        StatesUtils.ExitBuilding(Owner, HUNGRY);
        Owner.NeedsManager.WorkNeedOverride = false;
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion


    #region // Extention Functions

    void Eat(Food food)
    {
        float bite = Owner.AgentsSharedParameters.BiteSize * Time.deltaTime;

        food.FoodValue -= bite;
        Owner.ResourcesDataControllerRef.UpdateResourceProduction(FOOD, -bite);

        Owner.Food += bite;
    }

    #endregion
}
