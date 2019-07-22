using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Pathfinding;

public class Agent : MonoBehaviour
{
    #region // Agent's States
    [Header("States")]
    public bool JustBorn;
    public bool searching = false;
    public bool eating = false;
    public bool sleeping = false;
    public bool hasArraived = false;
    public bool wantsToMate = false;
    public bool foundMate = false;
    public bool working;
    public bool InBuilding;
    #endregion


    #region // Statistics
    [Header("Statistics")]
    public float distToPoint;

    public string MostUrgentNeed = ("null");
    
    
    public float CurrentAge { get; set; }
    public float Food { get; set; } = 1f;
    public float Energy { get; set; } = 1f;
    public float ReproductiveMultiplier { get; set; } = 0f;
    #endregion


    #region // Searching Variables
    public float MaxSearchTime = 7f;
    public float FoodSearchTime;
    public float RemapedFoodSearchTime;
    public float WorkSearchTime = 0f;
    public float RemapedWorkSearchTime;
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

    #endregion



    #region // General Variables
    public AgentsSharedParameters AgentsSharedParameters;


    GameObject globalStats;

    ResourcesDataController resourcesDataController;
    float timer = 0f;
    Vector3 SearchPoint;
    Vector3 _birthPosition;

    [HideInInspector]
    public Vector3 BuildingEnterPosition;

    [HideInInspector]
    public GameObject ClosestFood = null;

    [HideInInspector]
    public GenericBuilding CurrentWorkplace = null;

    [HideInInspector]
    public GameObject ClosestMate = null;

    [HideInInspector]
    public AgentMemory AgentMemory;

    [HideInInspector]
    public AgentNeedsManager NeedsManager;

    [HideInInspector]
    public AgentDecisionMaker DecisionMaker;

    #endregion


    #region // AStar Variables
    AIPath AstarAiPath;
    AIDestinationSetter aiDestinationSetter;
    [HideInInspector]
    public GameObject DestinationTarget;
    public GameObject DestinationTargetPrefab;
    #endregion


    #region // Needs index
    [HideInInspector]
    public int MostUrgentNeedByIndex;

    private const int NULLNEED = 0;
    private const int HUNGRY = 1;
    private const int TIRED = 2;
    private const int WORK = 3;
    private const int HORNY = 4;
    #endregion

    #region // State Machine
    [HideInInspector]
    public AgentStateMachine StateMachine;

    public enum StatesEnum { BaseState, SearchingWork, MovingToWork, Working }
    //StatesEnum statesEnum;

    public Dictionary<StatesEnum, IAgentState> States = new Dictionary<StatesEnum, IAgentState>();

    #endregion

    

    //public Dictionary<string, float> TraitsDic = new Dictionary<string, float>();
    [HideInInspector]
    public float[] Traits;

    // Start is called before the first frame update
    void Awake()
    {
        globalStats = GameObject.Find("GlobalStats");

        resourcesDataController = GameObject.Find("GameManager").GetComponent<ResourcesDataController>();

        AgentMemory = GetComponent<AgentMemory>();

        NeedsManager = GetComponent<AgentNeedsManager>();

        StateMachine = GetComponent<AgentStateMachine>();

        DecisionMaker = GetComponent<AgentDecisionMaker>();

        AgentUtils.SetColor(gameObject, 2, AgentColor);

        AgentUtils.InitializeTraits(this, Traits);

        #region // Pathfinding
        GameObject targetParent = GameObject.Find("AgentsTargets");
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        DestinationTarget = Instantiate(DestinationTargetPrefab, targetParent.transform);
        DestinationTarget.name = gameObject.name + ("Target");
        //DestinationTarget.transform.parent = targetParent.transform;
        AstarAiPath = gameObject.GetComponent(typeof(AIPath)) as AIPath;
        AstarAiPath.maxSpeed = AgentsSharedParameters.AgentSpeed;
        #endregion
    }

    void Update()
    {
        if (JustBorn) InitializeNewBorn();

        MostUrgentNeedByIndex = NeedsManager.FindMostUrgentNeed(this);

        StateMachine.Execute();
    }

    void LateUpdate()
    {
        CurrentAge = CurrentAge + Time.deltaTime;
        
        

        if(FoodSearchTime > MaxSearchTime * 2)
        {
            FoodSearchTime = 0f;
        }
        if (WorkSearchTime > MaxSearchTime * 2)
        {
            WorkSearchTime = 0f;
        }

        if(FoodSearchTime > MaxSearchTime - (MaxSearchTime * 0.15f))
        {
            ReproductiveMultiplier = ReproductiveMultiplier * 0f;
            FoodSearchTime += Time.deltaTime;
        }
        if (WorkSearchTime > MaxSearchTime - (MaxSearchTime * 0.15f))
        {
            WorkSearchTime += Time.deltaTime;
        }

        globalStats.GetComponent<GlobalStats>().GodForce += 0.05f * Time.deltaTime;

        ReproductiveMultiplier = ReproductiveMultiplier + ((Time.deltaTime) * 0.1f);
        ReproductiveMultiplier = UnityEngine.Mathf.Clamp(ReproductiveMultiplier, AgentsSharedParameters.MinReproductiveUrgeMultiplier, AgentsSharedParameters.MaxReproductiveMultiplier);

        if (Food <= 3f)
        {
            if(InBuilding) CurrentWorkplace.AgentsWorking -= 1;

            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Hunger");
            Destroy(gameObject);
        }
        if (Energy <= 3f)
        {
            if (InBuilding) CurrentWorkplace.AgentsWorking -= 1;
            
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Exhaustion");
            Destroy(gameObject);
        }
        if (CurrentAge >= MaxAge)
        {
            if (InBuilding) CurrentWorkplace.AgentsWorking -= 1;
            
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Old Age");
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        StateMachine.currentState.OnTriggerStay(collider);

        if ((collider.tag == "Agent") && (MostUrgentNeedByIndex == HORNY))
        {
            if(collider.GetComponent<Agent>().wantsToMate == true)
            {
                Agent mate = collider.transform.GetComponent<Agent>();
                if (CurrentAge + Energy > mate.CurrentAge + mate.Energy)
                {
                    AgentReproduction.Reproduce(this, mate);
                }
            }  
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, AgentsSharedParameters.SearchRadius);
    }

    string ConvertNeedIndexToString(int needIndex)
    {
        string needString;

        if (needIndex == HUNGRY)
        {
            needString = ("Hungry");
            return needString;
        }
        if (needIndex == TIRED)
        {
            needString = ("Tired");
            return needString;
        }
        if (needIndex == WORK)
        {
            needString = ("Ready For Work");
            return needString;
        }
        if (needIndex == HORNY)
        {
            needString = ("Horney");
            return needString;
        }
        else
        {
            needString = ("No Need Found");
            return needString;
        }
    }

    public void MoveTo(GameObject target)
    {
        aiDestinationSetter.target = target.transform;

        float velocity = GetComponent<Rigidbody2D>().velocity.magnitude;

        Energy -= (EnergyConsumption * Size * Time.deltaTime * AgentsSharedParameters.ConsumptionScale);
        Food -= (FoodConsumption * Size * Time.deltaTime * AgentsSharedParameters.ConsumptionScale);
    }

    void Eat(GameObject food)
    {
        eating = true;
        float bite;

        SpriteRenderer renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        renderer.material.color = Color.yellow;

        //float _foodvalue = _food.GetComponent<Food>().FoodValue;
        bite = (AgentsSharedParameters.BiteSize * Time.deltaTime);

        Food = Food + bite;

        food.GetComponent<Food>().FoodValue -= bite;

        resourcesDataController.UpdateResourceProduction("Food", -bite);
    }

    void Sleep()
    {
            timer = timer + Time.deltaTime;
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.blue;
            
            Energy = Energy + (AgentsSharedParameters.SleepEfficiency * Time.deltaTime);

            if (Energy < AgentsSharedParameters.AwakeThreshold)
            {
                sleeping = true;
            }
            if (Energy >= AgentsSharedParameters.AwakeThreshold)
            {
                sleeping = false;
                hasArraived = false;
                _renderer.material.SetColor("_Color", Color.green);
            }
    }

    public void SetBuildingState(bool building)
    {
        InBuilding = !building;

        transform.GetChild(0).gameObject.SetActive(building);
        transform.GetChild(1).gameObject.SetActive(building);
        transform.GetChild(2).gameObject.SetActive(building);
        transform.GetChild(3).gameObject.SetActive(building);

        GetComponent<AIPath>().enabled = building;
        GetComponent<AIDestinationSetter>().enabled = building;
        GetComponent<SimpleSmoothModifier>().enabled = building;
        GetComponent<Collider2D>().enabled = building;
        GetComponent<Rigidbody2D>().simulated = building;
    }

    void InitializeStateMachine()
    {
        States.Add(StatesEnum.BaseState, new AgentStateBase(this));

        States.Add(StatesEnum.SearchingWork, new AgentStateSearchingWorkplace(this));
        States.Add(StatesEnum.MovingToWork, new AgentStateMovingToWork(this));
        States.Add(StatesEnum.Working, new AgentStateWorking(this));

        StateMachine.ChangeState(States[StatesEnum.BaseState]);
    }

    public GameObject InstantiateBaby(Vector3 birthLocation)
    {
        GameObject baby = Instantiate(AgentsSharedParameters.AgentPrefab, birthLocation, Quaternion.identity);

        baby.name = "Agent (clone) " + UnityEngine.Random.Range(1000, 1999);

        baby.GetComponent<Agent>().JustBorn = true;

        return baby;
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
        AgentUtils.SetColor(gameObject, 2, AgentColor);

        Energy = 90f;
        Food = 20f;

        FoodSearchTime = 0f;
        CurrentAge = 0f;
        ReproductiveMultiplier = 0f;
        wantsToMate = false;
        NeedsManager.NeedsValues[HORNY] = 0f;
        JustBorn = false;
    }



    #region // OLD void ExecuteDecision()
    /*
    void ExecuteDecision()
    {
        distToPoint = Math.Abs(Vector3.Distance(transform.position, DestinationTarget.transform.position)); //Utility
        if (distToPoint < 2f)
        {
            searching = false;
            hasArraived = true;
        }


        // If need is Work, Go To Work
        if (MostUrgentNeedByIndex == WORK)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.green;

            wantsToMate = false;
            eating = false;
            sleeping = false;
            hasArraived = false;
            //FoodSearchTime = 0f;


            if (working == true)
            {
                //aiDestinationSetter.target = transform;
                Work(CurrentWorkplace.gameObject);
            }

            // if agent is not working
            if (working == false)
            {
                CurrentWorkplace = AgentUtils.FindClosestObject(gameObject, SearchRadius, "Work"); //find Closest Workplace

                // found close workplace //
                if (CurrentWorkplace != null && CurrentWorkplace.GetComponent<GenericBuilding>().WorkersNeeded)
                {
                    WorkSearchTime = 0f;
                    //move to workplace
                    MoveTo(ClosestWork);
                    searching = false;
                }

                // Has not found close work place //
                if (ClosestWork == null || ClosestWork.GetComponent<GenericBuilding>().WorkersNeeded == false)
                {
                    WorkSearchTime = WorkSearchTime + Time.deltaTime;

                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        //get random location
                        Vector3 _location = AgentUtils.RandomLocation(SearchRadius);
                        _location += transform.position;

                        //Check if location is valid
                        bool PositionValid = Physics2D.CircleCast(_location, 0.55f, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                        if (PositionValid)
                        {
                            SearchPoint = _location;
                            searching = true;
                            DestinationTarget.transform.position = SearchPoint;
                            MoveTo(DestinationTarget);
                        }

                        if (PositionValid == false)
                        {
                            searching = false;
                        }
                    }

                    // if you are searching continue search
                    if (searching == true)
                    {
                        DestinationTarget.transform.position = SearchPoint;
                        MoveTo(DestinationTarget);
                    }
                }

            }

        }

        if (MostUrgentNeedByIndex == HUNGRY)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.yellow;

            wantsToMate = false;
            working = false;
            sleeping = false;
            hasArraived = false;
            //workSearchTime = 0f;

            if (eating == true)
            {
                aiDestinationSetter.target = transform;
                if (ClosestFood == null)
                {
                    eating = false;
                }
            }

            // if agent is not eating
            if (eating == false)
            {
                ClosestFood = AgentUtils.FindClosestObject(gameObject, SearchRadius, "Food"); //find Closest Food

                // found close Food //
                if (ClosestFood != null)
                {
                    FoodSearchTime = 0f;
                    //move to Food
                    MoveTo(ClosestFood);
                    searching = false;
                }

                // Has not found close Food //
                if (ClosestFood == null)
                {
                    FoodSearchTime = FoodSearchTime + Time.deltaTime;

                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        //get random location
                        Vector3 _location = AgentUtils.RandomLocation(SearchRadius);
                        _location = transform.position + _location;
                        //Check if location is valid
                        bool PositionValid = Physics2D.CircleCast(_location, 0.55f, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                        if (PositionValid)
                        {
                            SearchPoint = _location;
                            searching = true;
                            DestinationTarget.transform.position = SearchPoint;
                            MoveTo(DestinationTarget);
                        }

                        if (PositionValid == false)
                        {
                            searching = false;
                        }
                    }

                    // if you are searching continue search
                    if (searching == true)
                    {
                        DestinationTarget.transform.position = SearchPoint;
                        MoveTo(DestinationTarget);
                    }
                }

            }

        }

        if (MostUrgentNeedByIndex == TIRED)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.cyan;

            wantsToMate = false;
            working = false;
            eating = false;
            //FoodSearchTime = 0f;
            //workSearchTime = 0f;


            if (searching == false && sleeping == false)
            {
                //get random location
                Vector3 _location = AgentUtils.RandomLocation(SearchRadius);
                _location = transform.position + _location;
                //Check if location is valid
                bool PositionValid = Physics2D.CircleCast(_location, 0.55f, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                if (PositionValid)
                {
                    SearchPoint = _location;
                    searching = true;
                    DestinationTarget.transform.position = SearchPoint;
                    MoveTo(DestinationTarget);
                }

                if (PositionValid == false)
                {
                    searching = false;
                }
            }
            
            if(hasArraived == true)
            {
                searching = false;
                sleeping = true;
                Sleep();
            }
        }

        if (MostUrgentNeedByIndex == HORNY)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.magenta;

            wantsToMate = true;
            working = false;
            eating = false;
            sleeping = false;
            hasArraived = false;
            //FoodSearchTime = 0f;
            //workSearchTime = 0f;


            if (foundMate)
            {
                if(ClosestMate != null)
                {
                    if (ClosestMate.tag == ("GodAngel"))
                    {
                        if (ClosestMate.GetComponent<GodAngel>().wantsToMate)
                        {
                            searching = false;
                            MoveTo(ClosestMate);
                        }
                        if (ClosestMate.GetComponent<GodAngel>().wantsToMate == false)
                        {
                            ClosestMate = null;
                        }
                    }

                    else if (ClosestMate.tag == ("Agent"))
                    {
                        if (ClosestMate.GetComponent<Agent>().wantsToMate)
                        {
                            searching = false;
                            MoveTo(ClosestMate);
                        }
                        if (ClosestMate.GetComponent<Agent>().wantsToMate == false)
                        {
                            ClosestMate = null;
                        }
                    } 
                }
                if(ClosestMate == null)
                {
                    foundMate = false;
                }
            }

            if (foundMate == false) // dont have mate
            {
                ClosestMate = AgentUtils.FindClosestMate(gameObject, SearchRadius); // find closest mate

                // found agent
                if (ClosestMate != null) 
                {
                    if (ClosestMate.tag == ("GodAngel"))
                    {
                            searching = false;
                            MoveTo(ClosestMate);
                    }

                    else if (ClosestMate.tag == ("Agent"))
                    {

                            searching = false;
                            MoveTo(ClosestMate);
                    }
                }
                
                //didnt find another agent, find new search point
                if (ClosestMate == null)
                {
                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        Vector3 _location = AgentUtils.RandomLocation(SearchRadius);
                        _location = transform.position + _location;
                        bool PositionValid = Physics2D.CircleCast(_location, 0.55f, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));

                        if (PositionValid)
                        {
                            SearchPoint = _location;
                            searching = true;
                            DestinationTarget.transform.position = SearchPoint;
                            MoveTo(DestinationTarget);
                        }
                        if (PositionValid == false)
                        {
                            searching = false;
                        }
                    }

                    if (searching == true)
                    {
                        DestinationTarget.transform.position = SearchPoint;
                        MoveTo(DestinationTarget);
                    }
                }
            }
        }
    }*/
    #endregion
}

