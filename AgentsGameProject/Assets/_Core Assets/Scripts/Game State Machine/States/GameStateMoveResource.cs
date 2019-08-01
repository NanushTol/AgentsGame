using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMoveResource : IState
{
    LevelManager _owner;

    public GameStateMoveResource(LevelManager owner) { this._owner = owner; }

    public string StateName
    {
        get { return "Move Resource"; }
    }

    public void Enter()
    {
        _owner.ActiveState = LevelManager.StatesEnum.MoveResource;

        _owner.SelectObjectRef.SelectedResourceUi.SetActive(true);
        _owner.SelectObjectRef.SelectedControlPanelUi.SetActive(true);

        _owner.SelectObjectRef.GetResourceProperties();

        _owner.MoveObjectRef.CostIndicator.SetActive(true);
    }

    public void ExecuteState()
    {
        _owner.ResourcesDataControllerRef.ConstantUpdate();
        _owner.UserControlsRef.TimeContorl(KeyCode.Space);

        _owner.SelectObjectRef.GetResourceProperties();


        _owner.MoveObjectRef.GetSelectedData();
        _owner.MoveObjectRef.GetMousePos();
        _owner.MoveObjectRef.CalculateCost();
        _owner.MoveObjectRef.PlaceIndicator();

        if (Input.GetMouseButtonDown(0))
            _owner.MoveObjectRef.PlaceObject();

        //else if (Input.GetKeyDown(KeyCode.Escape))
        //    _owner.StateMachineRef.ChangeState(_owner.States[LevelManager.StatesEnum.ResourceSelected]);

        _owner.SelectObjectRef.SelectedUpdate();
    }

    public void Exit()
    {
        _owner.SelectObjectRef.SelectedResourceUi.SetActive(false);
        _owner.SelectObjectRef.SelectedControlPanelUi.SetActive(false);

        _owner.MoveObjectRef.CostIndicator.SetActive(false);

        _owner.SelectObjectRef.PlaceIndicator(new Vector3(0f, -1000f, 0f));
    }
}
