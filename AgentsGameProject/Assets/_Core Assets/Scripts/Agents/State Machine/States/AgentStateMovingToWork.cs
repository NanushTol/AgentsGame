using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AgentStateMovingToWork : IAgentState
{
    public Agent Owner;

    public AgentStateMovingToWork(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Going To Work"; }

    }
    public Color StateColor
    {
        get { return Color.green; }
    }

    int _dicIndex;


    #region // State Functions

    public void Enter()
    {
        _dicIndex = 0;

        Owner.CurrentWorkplace = Owner.AgentMemory.Workplaces.ElementAt(_dicIndex).Key;

        StatesUtils.MoveTo(Owner, Owner.CurrentWorkplace.gameObject);

        Owner.ActiveState = Agent.StatesEnum.MovingToWork;
        Owner.State = StateName;
    }

    public void ExecuteState()
    {
        if (StatesUtils.ValidateState(Owner, WORK))
        {
            if (!Owner.CurrentWorkplace.WorkersNeeded)
            {
                if (_dicIndex < Owner.AgentMemory.Workplaces.Count)
                {
                    _dicIndex++;

                    Owner.CurrentWorkplace = Owner.AgentMemory.Workplaces.ElementAt(_dicIndex).Key;

                    StatesUtils.MoveTo(Owner, Owner.CurrentWorkplace.gameObject);
                }
                else
                {
                    // Set Work Need to Zero 
                    // Cuases the agent to find the next most urgent need
                    Owner.NeedsManager.WorkNeedOverride = true;

                    Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
                }
            } 
        }

        else
            Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.BaseState]);
    }

    public void Exit()
    {
        
    }

    public void OnTriggerStay(Collider2D collider)
    {
        if(collider.gameObject == Owner.CurrentWorkplace.gameObject)
        {
            StatesUtils.EnterBuilding(Owner, WORK);
            Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.Working]);
        }
    }

    #endregion

    #region // Extention Functions


    #endregion
}
