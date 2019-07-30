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
    
    [Header("Agent UI Panels")]
    public GameObject DdAgentType;
    public GameObject agentPropertiesUi;

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
    public GameObject SelectedAgent;
    [HideInInspector]
    public bool AgentInBuilding;



    #endregion


    #region // Buildings Properties
    [Header("Building UI Panel")]
    public GameObject WorkPropertiesUi;

    [Header("Building Variables")]

    public StringVariable BuildingType;
    public StringVariable ResourceType;
    public FloatVariable ResourceAmount;
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

    bool _agentIsSelected = false;
    bool _workIsSelected = false;

    Agent _agent;
    GenericBuilding _building;

    GameStates gameStates;

    [HideInInspector]
    GameObject selectionIndicator;
    [HideInInspector]
    public GameObject SelectedObject;

    int mask;

    // Start is called before the first frame update
    void Awake()
    {
        selectionIndicator = GameObject.Find("SelectionIndicator");
        gameStates = GameObject.Find("GameManager").GetComponent<GameStates>();
        
        int BuildingsLayer = 1 << LayerMask.NameToLayer("Building");
        int agentLayer = 1 << LayerMask.NameToLayer("Agent");
        int SleepPlacesLayer = 1 << LayerMask.NameToLayer("SleepPlace");
        int resourceLayer = 1 << LayerMask.NameToLayer("Resource");

        mask = BuildingsLayer | agentLayer | SleepPlacesLayer | resourceLayer;
    }


    // Update is called once per frame
    void Update()
    {
        if (gameStates.BuildingGameState == false)
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= DisplayUpdateTime)
            {
                UpdatePropertiesUi.Raise();
            }

            if (Input.GetMouseButtonDown(0) && !IsMouseOverUi())
            {
                RaycastHit2D hit = CastRay();

                GetSelectedObjectType(hit);
            }

            if (SelectedObject)
            {
                if (_agentIsSelected)
                {
                    GetAgentProperties();

                    if (!AgentInBuilding)
                    {
                        PlaceIndicator(SelectedObject.transform.position);
                    }
                    else if (AgentInBuilding)
                    {
                        PlaceIndicator(new Vector3(0f, -100f, 0f));
                    }
                }

                else if (_workIsSelected)
                {
                    GetBuildingProperties();
                    PlaceIndicator(SelectedObject.transform.position);
                }

                else PlaceIndicator(SelectedObject.transform.position);
            } 
        }
    }

    void PlaceIndicator(Vector3 _position)
    {
        selectionIndicator.transform.position = _position;
    }

    private bool IsMouseOverUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void GetAgentProperties()
    {
        AgentInBuilding = _agent.InBuilding;

        AgentAge.SetValue(_agent.CurrentAge);
        AgentType.SetValue(_agent.AgentType);
        StateName.SetValue(_agent.State);
        AgentHunger.SetValue(_agent.NeedsManager.NeedsValues[HUNGRY]);
        AgentHorny.SetValue(_agent.NeedsManager.NeedsValues[HORNY]);
        AgentEnergy.SetValue(_agent.Energy / 100f);
    }

    void GetBuildingProperties()
    {
        BuildingType.SetValue(_building.BuildingType.name);
        BuildingActive.SetValue(_building.UserOverrideBuildingActive);
        BuildingProduction.SetValue(_building.addedValue);
        WorkingAgents.SetValue(_building.AgentsWorking);
        EnergyUpkeep.SetValue(_building.BuildingType.EnergyUpkeep);
        WaterUpkeep.SetValue(_building.BuildingType.WaterUpkeep);
        StoneUpkeep.SetValue(_building.BuildingType.StoneUpkeep);
        WoodUpkeep.SetValue(_building.BuildingType.WoodUpkeep);
        MineralsUpkeep.SetValue(_building.BuildingType.MineralUpkeep);

        if (_building.Resource != null)
        {
            ResourceType.SetValue(_building.Resource.typeOfResource.ToString());
            RemainingResource.SetValue(_building.ResourceAmount);
            ResourceColor.SetColor(_building.Resource.transform.GetChild(2).GetComponent<SpriteRenderer>().color);
        }
    }

    RaycastHit2D CastRay()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Physics2D.Raycast(mousePos, Camera.main.transform.forward, 15f, mask);
    }

    void GetSelectedObjectType(RaycastHit2D hit)
    {
        // if Raycast hit somthing
        if (hit)
        {
            SelectedObject = hit.transform.gameObject;

            string tag = SelectedObject.tag;

            switch (tag)
            {
                case "Agent":
                    agentPropertiesUi.SetActive(true);
                    WorkPropertiesUi.SetActive(false);
                    _agentIsSelected = true;
                    _workIsSelected = false;
                    SelectedAgent = SelectedObject;
                    _agent = SelectedObject.GetComponent<Agent>();
                    DdAgentType.GetComponent<GnobTypeDropdown>().UpdateType();
                    GetAgentProperties();
                    break;

                case "Work":
                    WorkPropertiesUi.SetActive(true);
                    agentPropertiesUi.SetActive(false);
                    _agentIsSelected = false;
                    _workIsSelected = true;
                    _building = SelectedObject.GetComponent<GenericBuilding>();
                    GetBuildingProperties();
                    break;

                //case "SleepingPlace":
                //    agentPropertiesUi.SetActive(false);
                //    _agentIsSelected = false;
                //    break;

                //case "Food":
                //    agentPropertiesUi.SetActive(false);
                //    _agentIsSelected = false;
                //    break;
            }

        }

        // No Hit
        else
        {
            SelectedObject = null;
            PlaceIndicator(new Vector3(0f, -100f, 0f));
            agentPropertiesUi.SetActive(false);
            WorkPropertiesUi.SetActive(false);
            _workIsSelected = false;
            _agentIsSelected = false;
        }
    }
}

