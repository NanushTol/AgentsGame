using System;
using UnityEngine;

public interface IAgentState
{
    String StateName { get; }
    Color StateColor { get; }

    void Enter();

    void ExecuteState();

    void Exit();

    void OnTriggerStay(Collider2D collider);
}
