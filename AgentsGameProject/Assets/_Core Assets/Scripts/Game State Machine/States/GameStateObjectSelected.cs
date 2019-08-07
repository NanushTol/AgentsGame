using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateObjectSelected : IState
{
    public LevelManager _owner;

    public GameStateObjectSelected(LevelManager owner) { this._owner = owner; }

    public string StateName
    {
        get { return "Object Selected"; }
    }

    public void Enter()
    {
        _owner.ActiveState = LevelManager.StatesEnum.ObjectSelected;
        _owner.SelectObjectRef.ActivateUi();
        _owner.SelectObjectRef.GetSelectedObjectProperties();
    }

    public void ExecuteState()
    {
        _owner.ResourcesDataControllerRef.ConstantUpdate();
        _owner.UserControlsRef.TimeContorl(KeyCode.Space);

        _owner.SelectObjectRef.GetSelectedObjectProperties();

        _owner.SelectObjectRef.PlaceIndicator(_owner.SelectObjectRef.SelectedObject.transform.position);

        _owner.SelectObjectRef.GetSelection();

        if (Input.GetKeyDown(KeyCode.Escape))
            _owner.StateMachineRef.ChangeState(_owner.States[LevelManager.StatesEnum.BaseState]);

        _owner.SelectObjectRef.SelectedUpdate();
    }

    public void Exit()
    {
        _owner.SelectObjectRef.SelectedAgentUi.SetActive(false);
        _owner.SelectObjectRef.SelectedBuildingUi.SetActive(false);
        _owner.SelectObjectRef.SelectedResourceUi.SetActive(false);
        _owner.SelectObjectRef.SelectedControlPanelUi.SetActive(false);

        _owner.SelectObjectRef.PlaceIndicator(new Vector3(0f, -1000f, 0f));
    }
}
