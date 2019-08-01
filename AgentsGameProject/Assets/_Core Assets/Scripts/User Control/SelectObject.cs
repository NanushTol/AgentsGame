using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using static Constants;

public class SelectObject : MonoBehaviour
{
    public GameEvent UpdatePropertiesUi;
    public float DisplayUpdateTime;
    float _elapsedTime;

    public GameObject SelectedControlPanelUi;
    
    [Header("Agent UI Panels")]
    public GameObject DdAgentType;
    public GameObject SelectedAgentUi;

    #region // Agents Properties
    [Header("Agent Variables")]

    public StringVariable AgentName;
    public StringVariable AgentType;
    public StringVariable StateName;

    public FloatVariable AgentAge;
    public FloatVariable AgentHunger;
    public FloatVariable AgentHorny;
    public FloatVariable AgentEnergy;

    [HideInInspector]
    public float[] AgentProperties = new float[11];
    
    [HideInInspector]
    public bool AgentInBuilding;



    #endregion


    #region // Buildings Properties
    [Header("Building UI Panel")]
    public GameObject SelectedBuildingUi;
    public GameObject SelectedResourceUi;


    [Header("Building Variables")]

    public StringVariable BuildingType;
    public StringVariable ResourceType;
    public ColorVariable ResourceColor;
    public BoolVariable BuildingActive;

    public FloatVariable BuildingProduction;
    public FloatVariable RemainingResource;
    public FloatVariable WorkingAgents;

    public FloatVariable EnergyUpkeep;
    public FloatVariable WaterUpkeep;
    public FloatVariable StoneUpkeep;
    public FloatVariable WoodUpkeep;
    public FloatVariable MineralsUpkeep;

    public float[] BuildingProperties = new float[8];
    public float[] BuildingBarMaxValue = new float[2];

    #endregion

    [HideInInspector]
    public Agent Agent;
    [HideInInspector]
    public GenericBuilding Building;
    [HideInInspector]
    public Resource Resource;


    LevelManager LevelManagerRef;

    [HideInInspector]
    GameObject selectionIndicator;
    [HideInInspector]
    public GameObject SelectedObject;
    [HideInInspector]
    public int SelectedObjectType;

    public int Mask;

    // Start is called before the first frame update
    void Awake()
    {
        selectionIndicator = GameObject.Find("SelectionIndicator");
        LevelManagerRef = GameObject.Find("GameManager").GetComponent<LevelManager>();

        int BuildingsLayer = 1 << LayerMask.NameToLayer("Building");
        int agentLayer = 1 << LayerMask.NameToLayer("Agent");
        int SleepPlacesLayer = 1 << LayerMask.NameToLayer("SleepPlace");
        int resourceLayer = 1 << LayerMask.NameToLayer("Resource");

        Mask = BuildingsLayer | agentLayer | SleepPlacesLayer | resourceLayer;
    }


    public void SelectedUpdate()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= DisplayUpdateTime)
        {
            UpdatePropertiesUi.Raise();
        }
    }

    public void GetSelection()
    {
        if (Input.GetMouseButtonDown(0) && !GameUtils.IsMouseOverUi())
        {
            RaycastHit2D hit = GameUtils.CastRayFromMouse(Mask);

            GetSelectedObjectType(hit);
        }
    }

    public void PlaceIndicator(Vector3 _position)
    {
        selectionIndicator.transform.position = _position;
    }

    public void GetSelectedObjectType(RaycastHit2D hit)
    {
        // if Raycast hit somthing
        if (hit)
        {
            SelectedObject = hit.transform.gameObject;

            string tag = SelectedObject.tag;

            switch (tag)
            {
                case "Agent":
                    SelectedObjectType = SELECTED_AGENT;
                    Agent = SelectedObject.GetComponent<Agent>();
                    SelectedAgentUi.SetActive(true);
                    SelectedControlPanelUi.SetActive(true);
                    LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.ObjectSelected]);
                    break;

                case "Work":
                    SelectedObjectType = SELECTED_BUILDING;
                    Building = SelectedObject.GetComponent<GenericBuilding>();
                    SelectedBuildingUi.SetActive(true);
                    SelectedControlPanelUi.SetActive(true);
                    LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.ObjectSelected]);
                    break;

                case "Food":
                    SelectedObjectType = SELECTED_FOOD;
                    Building = SelectedObject.GetComponent<GenericBuilding>();
                    SelectedBuildingUi.SetActive(true);
                    SelectedControlPanelUi.SetActive(true);
                    LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.ObjectSelected]);
                    break;

                case "Energy":
                case "Stone":
                case "Wood":
                case "Mineral":
                    SelectedObjectType = SELECTED_RESOURCE;
                    Resource = SelectedObject.GetComponent<Resource>();
                    SelectedResourceUi.SetActive(true);
                    LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.ObjectSelected]);
                    break;
            }

        }

        // No Hit
        else
        {
            LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.BaseState]);
        }
    }

    public void GetSelectedObjectProperties()
    {
        switch (SelectedObjectType)
        {
            case SELECTED_AGENT:
                GetAgentProperties();
                break;

            case SELECTED_BUILDING:
                GetBuildingProperties();
                break;

            case SELECTED_FOOD:
                GetFoodBuildingProperties();
                break;

            case SELECTED_RESOURCE:
                GetResourceProperties();
                break;
        }
    }

    public void GetAgentProperties()
    {
        AgentAge.SetValue(Agent.CurrentAge);
        AgentType.SetValue(Agent.AgentType);
        StateName.SetValue(Agent.State);
        AgentHunger.SetValue(Agent.NeedsManager.NeedsValues[HUNGRY]);
        AgentHorny.SetValue(Agent.NeedsManager.NeedsValues[HORNY]);
        AgentEnergy.SetValue(Agent.Energy / 100f);
    }

    public void GetBuildingProperties()
    {
        BuildingType.SetValue(Building.BuildingType.name);
        BuildingActive.SetValue(Building.UserOverrideBuildingActive);
        BuildingProduction.SetValue(1 / Time.deltaTime * Building.AddedValue);
        WorkingAgents.SetValue(Building.AgentsWorking);
        EnergyUpkeep.SetValue(Building.BuildingType.EnergyUpkeep);
        WaterUpkeep.SetValue(Building.BuildingType.WaterUpkeep);
        StoneUpkeep.SetValue(Building.BuildingType.StoneUpkeep);
        WoodUpkeep.SetValue(Building.BuildingType.WoodUpkeep);
        MineralsUpkeep.SetValue(Building.BuildingType.MineralUpkeep);

        if (Building.Resource != null)
        {
            ResourceType.SetValue(Building.Resource.typeOfResource.ToString());
            RemainingResource.SetValue(Building.ResourceAmount);
            ResourceColor.SetColor(Building.Resource.transform.GetChild(2).GetComponent<SpriteRenderer>().color);
        }
    }

    public void GetResourceProperties()
    {
        BuildingType.SetValue(Resource.typeOfResource.ToString());
        RemainingResource.SetValue(Resource.Amount);
    }

    public void GetFoodBuildingProperties()
    {
        BuildingType.SetValue(Building.BuildingType.name);
        BuildingActive.SetValue(Building.UserOverrideBuildingActive);
        BuildingProduction.SetValue(1 / Time.deltaTime * Building.AddedValue);
        WorkingAgents.SetValue(Building.AgentsWorking);
        EnergyUpkeep.SetValue(Building.BuildingType.EnergyUpkeep);
        WaterUpkeep.SetValue(Building.BuildingType.WaterUpkeep);
        StoneUpkeep.SetValue(Building.BuildingType.StoneUpkeep);
        WoodUpkeep.SetValue(Building.BuildingType.WoodUpkeep);
        MineralsUpkeep.SetValue(Building.BuildingType.MineralUpkeep);

        RemainingResource.SetValue(Building.gameObject.GetComponent<Food>().FoodValue);
        ResourceType.SetValue("Food:");
    }
}

