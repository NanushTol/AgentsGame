using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateMachine : MonoBehaviour
{
    public IAgentState currentState { get; private set; }

    public void ChangeState(IAgentState newState)
    {
        if (currentState != null && currentState != newState)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public void Execute()
    {
        if (currentState != null) currentState.ExecuteState();
    }
}
