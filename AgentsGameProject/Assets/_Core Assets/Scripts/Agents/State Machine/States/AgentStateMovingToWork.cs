using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    private const int NULLNEED = 0;
    private const int HUNGRY = 1;
    private const int TIRED = 2;
    private const int WORK = 3;
    private const int HORNY = 4;

    int _dicIndex;


    #region // State Functions

    public void Enter()
    {
        _dicIndex = 0;

        Owner.CurrentWorkplace = Owner.AgentMemory.Workplaces.ElementAt(_dicIndex).Key;

        Owner.MoveTo(Owner.CurrentWorkplace.gameObject);
    }

    public void ExecuteState()
    {
        if (Owner.MostUrgentNeedByIndex == WORK)
        {
            if (!Owner.CurrentWorkplace.WorkersNeeded)
            {
                if (_dicIndex < Owner.AgentMemory.Workplaces.Count)
                {
                    _dicIndex++;

                    Owner.CurrentWorkplace = Owner.AgentMemory.Workplaces.ElementAt(_dicIndex).Key;

                    Owner.MoveTo(Owner.CurrentWorkplace.gameObject);
                }
                else
                {
                    // Stop Looking For Work
                    // set most urgent need TO next most urgent need
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
        EnterBuilding();
        Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.Working]);
    }

    #endregion

    #region // Extention Functions

    void EnterBuilding()
    {
        Owner.BuildingEnterPosition = Owner.transform.position;
        Owner.CurrentWorkplace.AgentsWorking += 1;
        Owner.SetBuildingState(true);
    }

    #endregion
}
