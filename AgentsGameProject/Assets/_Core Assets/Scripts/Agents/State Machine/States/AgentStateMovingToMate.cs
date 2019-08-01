using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateMovingToMate : IAgentState
{
    public Agent Owner;

    public AgentStateMovingToMate(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Going To Mate"; }

    }
    public Color StateColor
    {
        get { return Color.magenta; }
    }

    int _dicIndex;


    #region // State Functions

    public void Enter()
    {
        Owner.ActiveState = Agent.StatesEnum.MovingToMate;
        Owner.State = StateName;
    }

    public void ExecuteState()
    {
        if (StatesUtils.ValidateState(Owner, HORNY) && Owner.ChosenMate != null)
        {
            if (Owner.ChosenMate.GetComponent<Agent>().ActiveState == Agent.StatesEnum.MovingToMate)
            {
                    StatesUtils.MoveTo(Owner, Owner.ChosenMate);
            }
            else
            {
                Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.SearchingMate]);
            }
        }

        else if(StatesUtils.ValidateState(Owner, HORNY) && Owner.ChosenMate == null)
            Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.SearchingMate]);

        else
            Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
    }

    public void Exit()
    {
        Owner.NeedsManager.WorkNeedOverride = false;
        Owner.NeedsManager.FoodNeedOverride = false;
    }

    public void OnTriggerStay(Collider2D collider)
    {
        if(collider.gameObject == Owner.ChosenMate)
        {
            Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.Mating]);
        }
    }

    #endregion

    #region // Extention Functions


    #endregion
}
