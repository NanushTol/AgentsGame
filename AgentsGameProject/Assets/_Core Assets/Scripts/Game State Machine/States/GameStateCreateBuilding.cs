using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateCreateBuilding : IState
{
    LevelManager _owner;

    public GameStateCreateBuilding(LevelManager owner) { this._owner = owner; }


    public string StateName
    {
        get { return "Create Building"; }
    }

    public void Enter()
    {
        _owner.SelectObjectRef.enabled = false;
        _owner.ActiveState = LevelManager.StatesEnum.CreatingBuilding;
    }

    public void ExecuteState()
    {
        _owner.ResourcesDataControllerRef.ConstantUpdate();
        _owner.UserControlsRef.TimeContorl(KeyCode.Space);


        _owner.CreateBuildingRef.DisplayBuildingAtMousePosition();
        _owner.CreateBuildingRef.SelectLocation();
    }

    public void Exit()
    {
        
    }
}
