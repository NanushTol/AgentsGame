using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateMovingToSleep : IAgentState
{
    public Agent Owner;

    public AgentStateMovingToSleep(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Going To Sleep"; }

    }
    public Color StateColor
    {
        get { return Color.blue; }
    }

    int _dicIndex;


    #region // State Functions

    public void Enter()
    {
        Owner.ActiveState = Agent.StatesEnum.MovingToSleep;
        Owner.State = StateName;
    }

    public void ExecuteState()
    {
        if (StatesUtils.ValidateState(Owner, TIRED))
        {
            StatesUtils.MoveTo(Owner, Owner.CurrentSleepPlace.gameObject);
        }
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
        if(collider.gameObject == Owner.CurrentSleepPlace.gameObject)
        {
            StatesUtils.EnterBuilding(Owner, TIRED);
            Owner.StateMachineRef.ChangeState(Owner.States[Agent.StatesEnum.Sleeping]);
        }
    }

    #endregion

    #region // Extention Functions


    #endregion
}
