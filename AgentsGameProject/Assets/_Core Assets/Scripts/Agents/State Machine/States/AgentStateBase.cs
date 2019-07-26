using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateBase : IAgentState
{
    public Agent Owner;

    public AgentStateBase(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Base State"; }

    }
    public Color StateColor
    {
        get { return Color.gray; }
    }


    #region // State Functions

    public void Enter()
    {
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.BaseState;
    }

    public void ExecuteState()
    {
        switch (Owner.MostUrgentNeedByIndex)
        {
            case HUNGRY:
                Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.SearchingFood]);
                break;

            case TIRED:
                Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.SearchingSleep]);
                break;

            case WORK:
                Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.SearchingWork]);
                break;

            case HORNY:
                Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.SearchingMate]);
                break;
        }
    }

    public void Exit()
    {
        
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion
}
