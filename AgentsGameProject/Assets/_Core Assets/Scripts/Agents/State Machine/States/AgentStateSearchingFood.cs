using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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


        Owner.ChosenFoodPlace = null;

        for (int i = 0; i < Owner.AgentMemory.FoodPlaces.Count; i++)
        {
            //if entry Workplace - needs workers & building is working asign CurrentWorkplace
            if (Owner.AgentMemory.FoodPlaces.ElementAt(i).Key.FoodValue > 0f && Owner.AgentMemory.FoodPlaces.ElementAt(i).Key.FeedingVacancy)
            {
                Owner.ChosenFoodPlace = Owner.AgentMemory.FoodPlaces.ElementAt(i).Key;
                Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.MovingToFood]);
            }
        }
        
        if(Owner.ChosenFoodPlace == null)
        {
            Owner.NeedsManager.FoodNeedOverride = true;
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
