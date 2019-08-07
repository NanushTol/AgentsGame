using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;


public class MoveObject : MonoBehaviour
{
    public Grid GridRef;
    public MapCreator MapCreatorRef;

    public GameObject CostIndicator;
    public FloatVariable GfCostIndicatorValue;
    public GameEvent UpdateCostIndicator;
    public ResourcesDataController ResourcesDataControllerRef;
    public FloatVariable GfAmount;

    

    [HideInInspector]
    public GameObject ObjectToMove;

    SelectObject selectObjectRef;
    GenericBuilding building;
    Agent agent;
    
    int _selectedType;

    [HideInInspector]
    public Vector3 Position;
    [HideInInspector]
    public float GfCost;

    [HideInInspector]
    GameObject selectionIndicator;

    [HideInInspector]
    LevelManager LevelManagerRef;

    void Awake()
    {
        selectObjectRef = GameObject.Find("UserControls").GetComponent<SelectObject>();
        selectionIndicator = GameObject.Find("SelectionIndicator");
        LevelManagerRef = GameObject.Find("GameManager").GetComponent<LevelManager>();
    }

    public void EnterMoveState()
    {
        LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.MoveObject]);
    }

    public void GetSelectedData()
    {
        _selectedType = selectObjectRef.SelectedObjectType;
        ObjectToMove = selectObjectRef.SelectedObject;
    }

    public void GetMousePos()
    {
        RaycastHit2D hit = GameUtils.CastRayFromMouse(LayerMask.GetMask("Ground"));
        Vector3 hitPosition = hit.point;
        Position = GridRef.WorldToCell(hitPosition);
        Position.x += 0.5f;
        Position.y += 0.5f;
    }

    public void PlaceObject()
    {
        if (GfAmount.Value > GfCostIndicatorValue.Value)
        {
            selectObjectRef.SelectedObject.transform.position = Position;

            switch (_selectedType)
            {
                case SELECTED_BUILDING:

                    Vector3Int position = GridRef.WorldToCell(ObjectToMove.transform.position);

                    position.x += MapCreatorRef.MapWidth / 2;
                    position.y += MapCreatorRef.MapHeight / 2;

                    selectObjectRef.Building.UpdateNode(position, false);
                    break;
            }

            LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.ObjectSelected]);
            ResourcesDataControllerRef.UpdateResourceAmount(GODFORCE, -GfCostIndicatorValue.Value);
        }
    }

    public void CalculateCost()
    {
        GfCostIndicatorValue.SetValue(Mathf.Abs(Vector3.Distance(Position, ObjectToMove.transform.position)));
        UpdateCostIndicator.Raise();
    }

    public void PlaceIndicator()
    {
        selectionIndicator.transform.position = Position;
    }
}
