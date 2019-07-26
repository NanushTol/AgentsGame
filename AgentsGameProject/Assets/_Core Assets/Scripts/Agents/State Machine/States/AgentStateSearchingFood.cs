using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateSearchingFood : IAgentState
{
    public Agent Owner;

    public AgentStateSearchingFood(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Searching Food"; }

    }
    public Color StateColor
    {
        get { return Color.yellow; }
    }

    public string LayerMaskString = "Building";
    public string Tag = "Food";

    #region // State Functions

    public void Enter()
    {
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.SearchingWork;
    }

    public void ExecuteState()
    {
        AddNewFoodToMemory();

        Owner.DecisionMaker.ScoreItemsInDictionary(Owner.AgentMemory.FoodPlaces);

        Owner.AgentMemory.FoodPlaces = Owner.DecisionMaker.SortDictionaryByValues(Owner.AgentMemory.FoodPlaces);

        Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.MovingToFood]);
    }

    public void Exit()
    {
        
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion

    #region // Extention Functions

    void AddNewFoodToMemory()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Owner.transform.position, Owner.AgentsSharedParameters.SearchRadius, LayerMask.GetMask(LayerMaskString));

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Food"))
            {
                if (Owner.AgentMemory.FoodPlaces.ContainsKey(collider.GetComponent<Food>()) == false)
                {
                    Owner.AgentMemory.AddItemToDictionary(collider.GetComponent<Food>(), Owner.AgentMemory.FoodPlaces);
                }
            }
        }
    }

    #endregion
}
