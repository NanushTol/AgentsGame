using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        switch (Owner.MostUrgentNeedByIndex)
        {
            case WORK:
                Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.SearchingWork]);
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

    #region // Extention Functions

    

    #endregion
}
