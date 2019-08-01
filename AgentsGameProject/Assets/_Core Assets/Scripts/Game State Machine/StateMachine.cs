using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public IState CurrentState { get; private set; }
    public IState PreviousState { get; private set; }


    public void ChangeState(IState newState)
    {
        if (CurrentState != null && CurrentState != newState)
        {
            PreviousState = CurrentState;

            CurrentState.Exit();
        }

        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Execute()
    {
        if (CurrentState != null) CurrentState.ExecuteState();
    }
}
