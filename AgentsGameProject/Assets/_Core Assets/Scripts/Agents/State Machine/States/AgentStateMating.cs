using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateMating : IAgentState
{
    public Agent Owner;

    public AgentStateMating(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Mating"; }

    }
    public Color StateColor
    {
        get { return Color.magenta; }
    }


    #region // State Functions

    public void Enter()
    {
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.Mating;
    }

    public void ExecuteState()
    {
        Agent mate = Owner.ChosenMate.GetComponent<Agent>();

        if (Owner.CurrentAge + Owner.Energy > mate.CurrentAge + mate.Energy)
        {
            AgentReproduction.Reproduce(Owner, mate);
        }

        Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
    }

    public void Exit()
    {
        Owner.AgentMemory.PotentialMates.Clear();
        Owner.NeedsManager.WorkNeedOverride = false;
        Owner.NeedsManager.FoodNeedOverride = false;
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion


    #region // Extention Functions


    #endregion
}
