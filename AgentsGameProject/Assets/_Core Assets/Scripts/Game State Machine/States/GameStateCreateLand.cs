using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateCreateLand : IState
{
    LevelManager _owner;

    public GameStateCreateLand(LevelManager owner) { this._owner = owner; }

    CreateLand _createLand;

    public string StateName
    {
        get { return "Create Land & Water"; }
    }

    public void Enter()
    {
        _owner.SelectObjectRef.enabled = false;
        _owner.ActiveState = LevelManager.StatesEnum.CreatingLand;
        _createLand = _owner.CreateLandRef.GetComponent<CreateLand>();
    }

    public void ExecuteState()
    {
        _owner.ResourcesDataControllerRef.ConstantUpdate();
        _owner.UserControlsRef.TimeContorl(KeyCode.Space);


        _createLand.CreatingLand();
    }

    public void Exit()
    {
        
    }
}
