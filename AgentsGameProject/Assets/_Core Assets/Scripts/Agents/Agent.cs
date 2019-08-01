using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Pathfinding;
using static Constants;

public class Agent : MonoBehaviour
{
    #region // Agent's States
    [Header("States")]
    public string State;
    public bool JustBorn;
    public bool hasArraived = false;
    public bool InBuilding;
    #endregion

    #region // Statistics
    [Header("Statistics")]
    public string MostUrgentNeed = ("null");
    public float CurrentAge = 0f;
    public float Food = 1f;
    public float Energy = 1f;
    public float ReproductiveMultiplier = 0f;
    public float ReproductiveClock = 0f;
    #endregion

    #region // Traits
    [Header("Traits")]

    public string AgentType = "Builder";

    public Color AgentColor;

    public float MaxAge = 60f;

    public float ReproductiveUrge;

    public float FoodConsumption;

    public float EnergyConsumption;

    public float WorkingSpeed = 0f;

    public float Size;

    [HideInInspector]
    public float[] Traits;

    #endregion

    #region // General Variables
    public AgentsSharedParameters AgentsSharedParameters;
    public IntVariable AgentsBorn;

    float timer = 0f;

    GameObject _agentsParent;
    GameObject _agentTargetsParent;

    [HideInInspector]
    public ResourcesDataController ResourcesDataControllerRef;

    [HideInInspector]
    public int MostUrgentNeedByIndex;

    [HideInInspector]
    public Vector3 BuildingEnterPosition;

    [HideInInspector]
    public Food ChosenFoodPlace = null;

    [HideInInspector]
    public GenericBuilding CurrentWorkplace = null;

    [HideInInspector]
    public SleepPlace CurrentSleepPlace = null;

    [HideInInspector]
    public GameObject ChosenMate = null;

    [HideInInspector]
    public AgentMemory AgentMemory;

    [HideInInspector]
    public AgentNeedsManager NeedsManager;

    [HideInInspector]
    public AgentDecisionMaker DecisionMaker;

    #endregion

    #region // AStar Variables
    AIPath AstarAiPath;
    [HideInInspector]
    public AIDestinationSetter aiDestinationSetter;
    [HideInInspector]
    public GameObject DestinationTarget;
    public GameObject DestinationTargetPrefab;
    #endregion

    #region // State Machine
    [HideInInspector]
    public AgentStateMachine StateMachineRef;

    
    public enum StatesEnum { BaseState, SearchingWork, MovingToWork, Working,
        SearchingMate, MovingToMate, Mating,
        SearchingFood, MovingToFood, Eating,
        SearchingSleep, MovingToSleep, Sleeping}

    public StatesEnum ActiveState;

    public Dictionary<StatesEnum, IAgentState> States = new Dictionary<StatesEnum, IAgentState>();

    #endregion
    
    
    void Awake()
    {
        SetReferences();

        // Set body color
        AgentUtils.SetChildColor(gameObject, 2, AgentColor);

        InitializeTraits();

        InitializePathfinder();

        InitializeStateMachine();
    }

    void Start()
    {

        ResourcesDataControllerRef.Population.ApplyChange(1); // add 1 to population
        DestinationTarget.name = gameObject.name + (" Target");
    }

    void Update()
    {
        if (JustBorn) InitializeNewBorn();

        MostUrgentNeedByIndex = NeedsManager.FindMostUrgentNeed(this);

        StateMachineRef.Execute();
        // Set Horn Color To State Color
        AgentUtils.SetChildColor(gameObject, 0, StateMachineRef.currentState.StateColor);
    }

    void LateUpdate()
    {
        CurrentAge = CurrentAge + Time.deltaTime;

        CreateGodForce();
        
        UpdateReproductiveMultiplier();

        DeathCheck();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        StateMachineRef.currentState.OnTriggerStay(collider);
    }

    void InitializeTraits()
    {
        Traits = new float[6];

        Traits[0] = MaxAge;
        Traits[1] = ReproductiveUrge;
        Traits[2] = FoodConsumption;
        Traits[3] = EnergyConsumption;
        Traits[4] = WorkingSpeed;
        Traits[5] = Size;
    }

    void InitializeStateMachine()
    {
        States.Add(StatesEnum.BaseState, new AgentStateBase(this));

        States.Add(StatesEnum.SearchingWork, new AgentStateSearchingWorkplace(this));
        States.Add(StatesEnum.MovingToWork, new AgentStateMovingToWork(this));
        States.Add(StatesEnum.Working, new AgentStateWorking(this));
        States.Add(StatesEnum.SearchingMate, new AgentStateSearchingMate(this));
        States.Add(StatesEnum.MovingToMate, new AgentStateMovingToMate(this));
        States.Add(StatesEnum.Mating, new AgentStateMating(this));
        States.Add(StatesEnum.SearchingFood, new AgentStateSearchingFood(this));
        States.Add(StatesEnum.MovingToFood, new AgentStateMovingToFood(this));
        States.Add(StatesEnum.Eating, new AgentStateEating(this));
        States.Add(StatesEnum.SearchingSleep, new AgentStateSearchingSleepingPlace(this));
        States.Add(StatesEnum.MovingToSleep, new AgentStateMovingToSleep(this));
        States.Add(StatesEnum.Sleeping, new AgentStateSleeping(this));

        StateMachineRef.ChangeState(States[StatesEnum.BaseState]);
    }

    void InitializePathfinder()
    {
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        DestinationTarget = Instantiate(DestinationTargetPrefab, _agentTargetsParent.transform);
        
        AstarAiPath = gameObject.GetComponent(typeof(AIPath)) as AIPath;
        AstarAiPath.maxSpeed = AgentsSharedParameters.AgentSpeed;
    }

    void InitializeNewBorn()
    {
        MaxAge = Traits[0];
        ReproductiveUrge = Traits[1];
        FoodConsumption = Traits[2];
        EnergyConsumption = Traits[3];
        WorkingSpeed = Traits[4];
        Size = Traits[5];

        // Apply Agent Body Color
        AgentUtils.SetChildColor(gameObject, 2, AgentColor);

        Energy = 90f;
        Food = 20f;

        CurrentAge = 0f;
        ReproductiveMultiplier = 0f;
        NeedsManager.NeedsValues[HORNY] = 0f;
        JustBorn = false;
    }

    void SetReferences()
    {
        ResourcesDataControllerRef = GameObject.Find("GameManager").GetComponent<ResourcesDataController>();
        _agentsParent = GameObject.Find("Agents");
        _agentTargetsParent = GameObject.Find("AgentsTargets");

        AgentMemory = GetComponent<AgentMemory>();

        NeedsManager = GetComponent<AgentNeedsManager>();

        StateMachineRef = GetComponent<AgentStateMachine>();

        DecisionMaker = GetComponent<AgentDecisionMaker>();
    }

    void UpdateReproductiveMultiplier()
    {
        ReproductiveClock += Time.deltaTime;

        if (ReproductiveClock <= AgentsSharedParameters.ReproductiveRecoveryTime)
            ReproductiveMultiplier = AgentUtils.Remap(ReproductiveClock, 0f, AgentsSharedParameters.ReproductiveRecoveryTime, 0f, 1f);
    }

    void CreateGodForce()
    {
        // update GodForce Production
        ResourcesDataControllerRef.UpdateResourceProduction(GODFORCE, AgentsSharedParameters.GfPerSecond * Time.deltaTime);
    }

    void DeathCheck()
    {
        if (Food <= 1f)
        {
            Debug.Log("Agent Died of Hunger");
            KillAgent();   
        }
        if (Energy <= 1f)
        {
            Debug.Log("Agent Died of Exhaustion");
            KillAgent();
        }
        if (CurrentAge >= MaxAge)
        {
            Debug.Log("Agent Died of Old Age");
            KillAgent();
        }
    }

    void KillAgent()
    {
        if (InBuilding && ActiveState == StatesEnum.Working) CurrentWorkplace.AgentsWorking -= 1;
        if (InBuilding && ActiveState == StatesEnum.Sleeping) CurrentSleepPlace.SleepingAgents -= 1;
        if (InBuilding && ActiveState == StatesEnum.Eating) ChosenFoodPlace.FeedingAgents -= 1;
        ResourcesDataControllerRef.Population.ApplyChange(-1);
        Destroy(gameObject);
    }

    public GameObject InstantiateBaby(Vector3 birthLocation)
    {
        GameObject baby = Instantiate(AgentsSharedParameters.AgentPrefab, birthLocation, Quaternion.identity);
        baby.transform.parent = _agentsParent.transform;
        AgentsBorn.ApplyChange(1);

        baby.name = "Agent " + AgentsBorn.Value;

        baby.GetComponent<Agent>().JustBorn = true;

        return baby;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, AgentsSharedParameters.SearchRadius);
    }


}

