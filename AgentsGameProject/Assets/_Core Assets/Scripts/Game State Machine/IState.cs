using System;
using UnityEngine;

public interface IState
{
    String StateName { get; }

    void Enter();

    void ExecuteState();

    void Exit();
}
