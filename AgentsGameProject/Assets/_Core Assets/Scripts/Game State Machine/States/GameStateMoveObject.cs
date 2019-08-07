using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMoveObject : IState
{
    LevelManager _owner;

    public GameStateMoveObject(LevelManager owner) { this._owner = owner; }

    public string StateName
    {
        get { return "Move Object"; }
    }

    public void Enter()
    {
        _owner.States[LevelManager.StatesEnum.ObjectSelected].Enter();

        _owner.MoveObjectRef.CostIndicator.SetActive(true);
    }

    public void ExecuteState()
    {
        _owner.MoveObjectRef.GetSelectedData();
        _owner.MoveObjectRef.GetMousePos();
        _owner.MoveObjectRef.CalculateCost();

        if (Input.GetMouseButtonDown(0))
            _owner.MoveObjectRef.PlaceObject();

        else if (Input.GetKeyDown(KeyCode.Escape))
            _owner.StateMachineRef.ChangeState(_owner.States[LevelManager.StatesEnum.ObjectSelected]);

        else
            _owner.States[LevelManager.StatesEnum.ObjectSelected].ExecuteState();

        _owner.MoveObjectRef.PlaceIndicator();

    }

    public void Exit()
    {
        _owner.MoveObjectRef.CostIndicator.SetActive(false);

        _owner.States[LevelManager.StatesEnum.ObjectSelected].Exit();
    }
}
