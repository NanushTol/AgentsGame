using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateSleeping : IAgentState
{
    public Agent Owner;

    public AgentStateSleeping(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Sleeping"; }

    }
    public Color StateColor
    {
        get { return Color.blue; }
    }


    #region // State Functions

    public void Enter()
    {
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.Sleeping;
    }

    public void ExecuteState()
    {
        if (Owner.Food < Owner.AgentsSharedParameters.AwakeThreshold)
            Sleep();
        else
            Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
    }

    public void Exit()
    {
        StatesUtils.ExitBuilding(Owner, TIRED);
        Owner.NeedsManager.WorkNeedOverride = false;
        Owner.NeedsManager.FoodNeedOverride = false;
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion


    #region // Extention Functions

    void Sleep()
    {
        Owner.Energy += (Owner.AgentsSharedParameters.SleepEfficiency * Time.deltaTime);
    }

    #endregion
}
