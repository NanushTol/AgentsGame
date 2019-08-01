using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject CreateLandRef;

    [HideInInspector]
    public GameObject UserControlsGo;
    [HideInInspector]
    public UserControls UserControlsRef;
    [HideInInspector]
    public SelectObject SelectObjectRef;
    [HideInInspector]
    public ResourcesDataController ResourcesDataControllerRef;
    [HideInInspector]
    public StateMachine StateMachineRef;
    [HideInInspector]
    public CreateBuilding CreateBuildingRef;
    [HideInInspector]
    public MoveObject MoveObjectRef;

    public enum StatesEnum
    {
        BaseState, CreatingBuilding, CreatingLand, ObjectSelected, MoveResource
    }

    //[HideInInspector]
    public StatesEnum ActiveState;

    public Dictionary<StatesEnum, IState> States = new Dictionary<StatesEnum, IState>();

    void Awake()
    {
        SetReferences();
        InitializeStateMachine();
    }

    void Update()
    {
        StateMachineRef.Execute();
    }

    void InitializeStateMachine()
    {
        States.Add(StatesEnum.BaseState, new GameStateBase(this));
        States.Add(StatesEnum.CreatingBuilding, new GameStateCreateBuilding(this));
        States.Add(StatesEnum.CreatingLand, new GameStateCreateLand(this));
        States.Add(StatesEnum.ObjectSelected, new GameStateObjectSelected(this));
        States.Add(StatesEnum.MoveResource, new GameStateMoveResource(this));


        StateMachineRef.ChangeState(States[StatesEnum.BaseState]);
    }

    void SetReferences()
    {
        UserControlsGo = GameObject.Find("UserControls");
        UserControlsRef = UserControlsGo.GetComponent<UserControls>();
        SelectObjectRef = UserControlsGo.GetComponent<SelectObject>();
        MoveObjectRef = UserControlsGo.GetComponent<MoveObject>();
        ResourcesDataControllerRef = GetComponent<ResourcesDataController>();
        StateMachineRef = GetComponent<StateMachine>();
    }
}
