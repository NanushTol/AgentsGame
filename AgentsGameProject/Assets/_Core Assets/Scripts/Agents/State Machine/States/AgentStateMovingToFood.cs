using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateMovingToFood : IAgentState
{
    public Agent Owner;

    public AgentStateMovingToFood(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Going To Eat"; }

    }
    public Color StateColor
    {
        get { return Color.yellow; }
    }

    int _dicIndex;


    #region // State Functions

    public void Enter()
    {
        _dicIndex = 0;

        Owner.ActiveState = Agent.StatesEnum.MovingToFood;
        Owner.State = StateName;
    }

    public void ExecuteState()
    {
        if (StatesUtils.ValidateState(Owner, HUNGRY))
        {
            StatesUtils.MoveTo(Owner, Owner.ChosenFoodPlace.gameObject);

            if (Owner.ChosenFoodPlace.FeedingVacancy == false || Owner.ChosenFoodPlace.FoodValue < 2f)
            {
                

                if (Owner.AgentMemory.FoodPlaces.Count - 1 > _dicIndex)
                {
                    Owner.ChosenFoodPlace = Owner.AgentMemory.FoodPlaces.ElementAt(_dicIndex).Key;

                    StatesUtils.MoveTo(Owner, Owner.ChosenFoodPlace.gameObject);
                }
                else
                {
                    // Set Work Need to Zero 
                    // Cuases the agent to find the next most urgent need
                    Owner.NeedsManager.FoodNeedOverride = true;

                    Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
                }

                _dicIndex++;
            } 
        }

        else
            Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
    }

    public void Exit()
    {
        Owner.NeedsManager.WorkNeedOverride = false;
    }

    public void OnTriggerStay(Collider2D collider)
    {
        if(collider.gameObject == Owner.ChosenFoodPlace.gameObject)
        {
            if(Owner.ChosenFoodPlace.FeedingVacancy)
            {
                StatesUtils.EnterBuilding(Owner, HUNGRY);
                Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.Eating]);
            }
            else
                Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.SearchingFood]);
        }
    }

    #endregion

    #region // Extention Functions


    #endregion
}
