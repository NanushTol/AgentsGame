using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateBase : IState
{
    LevelManager _owner;

    public GameStateBase(LevelManager owner) { this._owner = owner; }

    public string StateName
    {
        get { return "Base State"; }
    }

    public void Enter()
    {
        _owner.SelectObjectRef.enabled = true;
        _owner.ActiveState = LevelManager.StatesEnum.BaseState;
    }

    public void ExecuteState()
    {
        _owner.ResourcesDataControllerRef.ConstantUpdate();
        _owner.UserControlsRef.TimeContorl(KeyCode.Space);
        _owner.UserControlsRef.LevelMainMenu(KeyCode.Escape);

        if (Input.GetMouseButtonDown(0) && !GameUtils.IsMouseOverUi())
        {
            RaycastHit2D hit = GameUtils.CastRayFromMouse(_owner.SelectObjectRef.Mask);

            _owner.SelectObjectRef.GetSelectedObjectType(hit);
        }
    }

    public void Exit()
    {
        
    }
}
