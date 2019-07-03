using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Pathfinding;

public class GodAngel : MonoBehaviour
{
    #region // Variables

    public float distToPoint;

    GameObject closestFood = null;
    GameObject closestWork = null;
    GameObject closestMate = null;

    AIPath AstarAiPath;
    AIDestinationSetter aiDestinationSetter;
    GameObject DestinationTarget;
    public GameObject DestinationTargetPrefab;

    public GameObject AgentPrefab;

    public Dictionary<string, float> TraitsDic = new Dictionary<string, float>();
    public float[] Traits;
    public bool JustBorn;

    float timer = 0f;

    private float[] Needs;
    private const int HUNGRY = 0;
    private const int TIRED = 1;
    private const int WORK = 2;
    private const int HORNEY = 3;

    [SerializeField]
    Vector3 SearchPoint;

    public float FoodSearchTime;
    public float RemapedSearchTime;

    [SerializeField]
    bool searching = false;
    [SerializeField]
    bool eating = false;
    [SerializeField]
    bool sleeping = false;
    [SerializeField]
    bool hasArraived = false;

    public bool working = false;

    public bool wantsToMate = false;

    public bool foundMate = false;

    [Header("Stats")]

    public string mostUrgentNeed = ("null");
    private int mostUrgentNeedIndex;
    [SerializeField]
    float readyForWork;
    [SerializeField]
    float hungry = 0.0f;
    [SerializeField]
    float tierd = 0.0f;
    [SerializeField]
    public float horney = 0.0f;

    public float currentAge;
    [SerializeField]
    float food = 1f;
    public float energy = 1f;
    public float reproductiveUrge = 0f;

    public string GoingForWork;
    public string GoingForFood;

    [Header("Traits")]
    public string MissionInLife = "Builder";
    public float MaxAge = 60f;
    public float MaxSearchTime = 7f;
    public float MaxEnergy = 100;
    public float MaxFood = 100;
    public float MaxReproductiveUrge = 1f;
    public float MinReproductiveUrge = 0f;
    public float SpeedCost = 0.3f;
    public float WorkFoodCost = 1.8f;
    public float WorkEnergyCost = 1.2f;
    public float AgentSpeed = 10f;
    public float SearchRadius = 8f;

    public float BiteSize = 2f;
    public float SleepEfficiency = 2f;

    public float FoodFullThreshold = 60f;
    public float AwakeThreshold = 70f;
    public float Mutaion = 0.15f;

    public Color GodAngelColor;

    [SerializeField]
    AnimationCurve foodToHunger;
    [SerializeField]
    AnimationCurve searchTimeToHunger;
    [SerializeField]
    AnimationCurve energyToTiredness;
    [SerializeField]
    AnimationCurve energyToReadyness;
    [SerializeField]
    AnimationCurve ageToHorney;

    GameObject globalStats;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        TraitsDic.Add("MaxAge", MaxAge);
        TraitsDic.Add("MaxEnergy", MaxEnergy);
        TraitsDic.Add("MaxFood", MaxFood);
        TraitsDic.Add("MaxReproductiveUrge", MaxReproductiveUrge);
        TraitsDic.Add("SpeedCost", SpeedCost);
        TraitsDic.Add("WorkFoodCost", WorkFoodCost);
        TraitsDic.Add("WorkEnergyCost", WorkEnergyCost);
        TraitsDic.Add("AgentSpeed", AgentSpeed);
        TraitsDic.Add("SearchRadius", SearchRadius);

        Traits = new float[9];
        for (int i = 0; i < 9; i++)
        {
            Traits[i] = TraitsDic.ElementAt(i).Value;
        }

        aiDestinationSetter = GetComponent<AIDestinationSetter>();

        GameObject targetParent = GameObject.Find("AgentsTargets");
        DestinationTarget = Instantiate(DestinationTargetPrefab, targetParent.transform);
        DestinationTarget.name = gameObject.name + ("Target");
        DestinationTarget.transform.parent = targetParent.transform;

        AstarAiPath = gameObject.GetComponent(typeof(AIPath)) as AIPath;
        AstarAiPath.maxSpeed = AgentSpeed;

        Needs = new float[4];

        globalStats = GameObject.Find("GlobalStats");
    }

    // Update is called once per frame
    void Update()
    {
        if (JustBorn == true)
        {
            //MaxAge = Traits[0];
            //MaxEnergy = Traits[1];
            //MaxFood = Traits[2];
            //MaxReproductiveUrge = Traits[3];
            //SpeedCost = Traits[4];
            //WorkFoodCost = Traits[5];
            //WorkEnergyCost = Traits[6];
            //AgentSpeed = Traits[7];
            //SearchRadius = Traits[8];
            energy = 100f;
            food = 70f;

            FoodSearchTime = 0f;
            currentAge = 0f;
            reproductiveUrge = 0.5f;
            wantsToMate = false;
            Needs[HORNEY] = 0f;
            JustBorn = false;
            //mostUrgentNeedIndex = HUNGRY;
            //closestWork = null;
        }

        AgentDecisionMaking();
        ExecuteDecision();
    }

    void LateUpdate()
    {
        currentAge = currentAge + Time.deltaTime;
        FoodSearchTime = FoodSearchTime + Time.deltaTime;
        globalStats.GetComponent<GlobalStats>().GodForce += 0.05f * Time.deltaTime;

        reproductiveUrge = reproductiveUrge + ((Time.deltaTime) * 0.1f);
        reproductiveUrge = UnityEngine.Mathf.Clamp(reproductiveUrge, MinReproductiveUrge, MaxReproductiveUrge);

        if (food <= 3f)
        {
            Destroy(gameObject);
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Hunger");
        }
        if (energy <= 3f)
        {
            Destroy(gameObject);
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Exhaustion");
        }
        if (currentAge >= MaxAge)
        {
            Destroy(gameObject);
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Old Age");
        }
    }

    private void OnTriggerStay2D(Collider2D _collider)
    {
        if ((_collider.tag == "Work") && (mostUrgentNeedIndex == WORK))
        {
            Work(WorkFoodCost, WorkEnergyCost, closestWork);
            working = true;
        }
        if (_collider.CompareTag("Food") && mostUrgentNeedIndex == HUNGRY)
        {
            if (food < FoodFullThreshold)
            {
                eating = true;
            }
            if (_collider.transform.gameObject == closestFood)
            {
                Eat(closestFood);
            }
            if (_collider.transform.gameObject != closestFood)
            {
                eating = false;
            }
            if (food >= FoodFullThreshold)
            {
                eating = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _collider)
    {
        if (_collider.CompareTag("Food"))
        {
            eating = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, SearchRadius);
    }

    string ConvertNeedIndexToString(int _needIndex)
    {
        string _needString;

        if (_needIndex == HUNGRY)
        {
            _needString = ("Hungry");
            return _needString;
        }
        if (_needIndex == TIRED)
        {
            _needString = ("Tired");
            return _needString;
        }
        if (_needIndex == WORK)
        {
            _needString = ("Ready For Work");
            return _needString;
        }
        if (_needIndex == HORNEY)
        {
            _needString = ("Horney");
            return _needString;
        }
        else
        {
            _needString = ("No Need Found");
            return _needString;
        }
    }

    GameObject FindClosestObject(Vector3 _agentPosition, float _searchRadius, string _layerMask)
    {
        float closestObjectDistance = 1000f;
        GameObject _closestObject = null;

        //get all object in a given radius
        Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(_agentPosition, _searchRadius, LayerMask.GetMask(_layerMask));

        foreach (Collider2D _object in _objectColliders) //Loop over the given object found
        {
            Vector3 objectPosition = _object.transform.position; //get object position
            Vector3 thisPosition = gameObject.transform.position;
            // find distance to object
            //Vector3 rawDistanceToObject = transform.position - objectPosition;
            float distanceToObject = Vector3.Distance(objectPosition, thisPosition);

            //check if distance is smaller the the closest one yet
            if (distanceToObject < closestObjectDistance)
            {
                if (_object.gameObject.name != this.gameObject.name)
                {
                    _closestObject = _object.gameObject; //current object is closest else continue
                    closestObjectDistance = distanceToObject;
                }
            }
        }

        return _closestObject;
    }

    GameObject FindClosestMate(Vector3 _agentPosition, float _searchRadius, string _layerMask)
    {
        float closestObjectDistance = 1000f;
        GameObject _closestObject = null;

        //get all object in a given radius
        Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(_agentPosition, _searchRadius, LayerMask.GetMask(_layerMask));

        foreach (Collider2D _object in _objectColliders) //Loop over the given object found
        {
            Vector3 objectPosition = _object.transform.position; //get object position
            Vector3 thisPosition = gameObject.transform.position;
            // find distance to object
            //Vector3 rawDistanceToObject = transform.position - objectPosition;
            float distanceToObject = Vector3.Distance(objectPosition, thisPosition);

            //check if distance is smaller the the closest one yet
            if (distanceToObject < closestObjectDistance)
            {
                if (_object.gameObject.name != this.gameObject.name && _object.GetComponent<Agent>().wantsToMate)
                {
                    _closestObject = _object.gameObject; //current object is closest else continue
                    closestObjectDistance = distanceToObject;
                }
            }
        }

        return _closestObject;
    }

    void MoveTo(GameObject _target)
    {
        aiDestinationSetter.target = _target.transform;

        float _velocity = GetComponent<Rigidbody2D>().velocity.magnitude;

        energy = energy - (_velocity * SpeedCost);
        food = food - (_velocity * SpeedCost);
    }

    void Work(float _workFoodCost, float _workEnergyCost, GameObject _closestWork)
    {
        working = true;
        float _workPlaceEfficiency = _closestWork.GetComponent<WorkPlace>().WorkEfficiency;
        float _workPlaceProduction = _closestWork.GetComponent<WorkPlace>().Production;

        //create Production
        _workPlaceProduction = (_workPlaceProduction + _workPlaceEfficiency) * Time.deltaTime;
        _closestWork.GetComponent<WorkPlace>().Production = _workPlaceProduction;

        //reduce Energy & Food
        food = food - (_workFoodCost * Time.deltaTime);
        energy = energy - (_workEnergyCost * Time.deltaTime);
    }

    void Eat(GameObject _food)
    {
        eating = true;
        SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _renderer.material.color = Color.yellow;

        float _foodvalue = _food.GetComponent<Food>().FoodValue;

        food = food + (BiteSize * Time.deltaTime);

        _food.GetComponent<Food>().FoodValue = _food.GetComponent<Food>().FoodValue - (BiteSize * Time.deltaTime);
    }

    void Sleep()
    {
        timer = timer + Time.deltaTime;
        SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _renderer.material.color = Color.blue;


        energy = energy + (SleepEfficiency * Time.deltaTime);

        if (energy < AwakeThreshold)
        {
            sleeping = true;
        }
        if (energy >= AwakeThreshold)
        {
            sleeping = false;
            hasArraived = false;
            _renderer.material.SetColor("_Color", Color.green);
        }
    }

    Vector3 RandomLocation(float _minX, float _maxX, float _minY, float _maxY)
    {
        Vector3 _location = new Vector3(UnityEngine.Random.Range(_minX, _maxX), UnityEngine.Random.Range(_minY, _maxY), 0);

        return _location;
    }

    public void AgentDecisionMaking()
    {
        if (eating)
        {
            Needs[HUNGRY] = 100;
            hungry = Needs[HUNGRY];
        }
        if (eating == false)
        {

            RemapedSearchTime = Remap(FoodSearchTime, 0f, MaxSearchTime, 0f, 1f);
            Needs[HUNGRY] = foodToHunger.Evaluate(food) - searchTimeToHunger.Evaluate(RemapedSearchTime);
            hungry = Needs[HUNGRY];
        }

        if (sleeping)
        {
            Needs[TIRED] = 100;
            tierd = Needs[TIRED];
        }
        if (sleeping == false)
        {
            Needs[TIRED] = energyToTiredness.Evaluate(energy);
            tierd = energyToTiredness.Evaluate(energy);
        }

        Needs[WORK] = energyToReadyness.Evaluate(energy);
        readyForWork = Needs[WORK];

        Needs[HORNEY] = ageToHorney.Evaluate(currentAge) * reproductiveUrge;
        horney = Needs[HORNEY];

        float _mostUrgentNeedvalue = 0.0f;
        int _mostUrgentNeedIndex = 100;

        int i = 0;
        foreach (float _need in Needs)
        {
            i++;
            if (_need > _mostUrgentNeedvalue)
            {
                _mostUrgentNeedvalue = _need;
                _mostUrgentNeedIndex = i - 1;
                mostUrgentNeed = ConvertNeedIndexToString(_mostUrgentNeedIndex);
                mostUrgentNeedIndex = _mostUrgentNeedIndex;
            }
        }
    }

    void ExecuteDecision()
    {
        distToPoint = Math.Abs(Vector3.Distance(transform.position, DestinationTarget.transform.position));
        if (distToPoint < 2f)
        {
            searching = false;
            hasArraived = true;
        }


        // If need is Work, Go To Work
        if (mostUrgentNeedIndex == WORK)
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
                aiDestinationSetter.target = transform;
            }

            // if agent is not working
            if (working == false)
            {
                closestWork = FindClosestObject(transform.position, SearchRadius, "Work"); //find Closest Workplace

                // found close workplace //
                if (closestWork != null && closestWork.GetComponent<WorkPlace>().WorkersNeeded)
                {
                    //move to workplace
                    MoveTo(closestWork);
                    searching = false;
                }

                // Has not found close work place //
                if (closestWork == null || closestWork.GetComponent<WorkPlace>().WorkersNeeded == false)
                {
                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        //get random location
                        Vector3 _location = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius);
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

        if (mostUrgentNeedIndex == HUNGRY)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.yellow;

            wantsToMate = false;
            working = false;
            sleeping = false;
            hasArraived = false;

            if (eating == true)
            {
                aiDestinationSetter.target = transform;
                if (closestFood == null)
                {
                    eating = false;
                }
            }

            // if agent is not eating
            if (eating == false)
            {
                closestFood = FindClosestObject(transform.position, SearchRadius, "Food"); //find Closest Food

                // found close Food //
                if (closestFood != null)
                {
                    FoodSearchTime = 0f;
                    //move to Food
                    MoveTo(closestFood);
                    searching = false;
                }

                // Has not found close Food //
                if (closestFood == null)
                {
                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        //get random location
                        Vector3 _location = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius);
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

        if (mostUrgentNeedIndex == TIRED)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.cyan;

            //FoodSearchTime = 0f;

            wantsToMate = false;
            working = false;
            eating = false;

            if (searching == false && sleeping == false)
            {
                //get random location
                Vector3 _location = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius);
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

            if (hasArraived == true)
            {
                searching = false;
                sleeping = true;
                Sleep();
            }
        }

        if (mostUrgentNeedIndex == HORNEY)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.magenta;

            //FoodSearchTime = 0f;

            wantsToMate = true;
            working = false;
            eating = false;
            sleeping = false;
            hasArraived = false;

            if (foundMate)
            {
                if (closestMate != null)
                {
                    searching = false;
                    MoveTo(closestMate);
                }
                if (closestMate == null)
                {
                    foundMate = false;
                }
            }

            if (foundMate == false) // dont have mate
            {
                closestMate = FindClosestMate(transform.position, SearchRadius, "Agent"); // find close agents

                // found agent
                if (closestMate != null)
                {
                    //check if he wants to mate
                    if (closestMate.transform.GetComponent<Agent>().wantsToMate)
                    {
                        MoveTo(closestMate);
                        searching = false;
                        foundMate = true;
                    }
                    if (closestMate.transform.GetComponent<Agent>().wantsToMate == false)
                    {
                        Vector3 _location = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius);
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
                }

                //didnt find another agent, find new search point
                if (closestMate == null)
                {
                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        Vector3 _location = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius);
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
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        float _remapedValue = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        return _remapedValue;
    }
}

