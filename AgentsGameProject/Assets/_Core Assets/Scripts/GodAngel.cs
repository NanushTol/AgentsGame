using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class GodAngel : MonoBehaviour
{
    #region // Variables

    public float distToPoint;

    GameObject closestFood = null;
    GameObject closestWork = null;
    GameObject closestMate = null;
    NavMeshAgent navMeshAgent;
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

    public float SearchTime;
    public float RemapedSearchTime;

    [SerializeField]
    bool searching = false;
    [SerializeField]
    bool eating = false;
    [SerializeField]
    bool sleeping = false;
    [SerializeField]
    bool hasArraived = false;

    bool working = false;

    public float HasArrivedThresh = 3f;

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

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        #region // Set Traits
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
        for(int i = 0; i < 9; i++)
        {
            Traits[i] = TraitsDic.ElementAt(i).Value;
        }
        #endregion

        #region // Set NavMesh
        navMeshAgent = gameObject.GetComponent(typeof(NavMeshAgent)) as NavMeshAgent;
        navMeshAgent.speed = AgentSpeed;
        #endregion

        Needs = new float[4];
    }

       // Update is called once per frame
    void Update()
    {
        if (JustBorn == true)
        {
            MaxAge = Traits[0];
            MaxEnergy = Traits[1];
            MaxFood = Traits[2];
            MaxReproductiveUrge = Traits[3];
            //SpeedCost = Traits[4];
            //WorkFoodCost = Traits[5];
            WorkEnergyCost = Traits[6];
            //AgentSpeed = Traits[7];
            //SearchRadius = Traits[8];
            energy = 100f;
            food = 70f;

            navMeshAgent.speed = AgentSpeed;
            currentAge = 0f;
            reproductiveUrge = 0f;
            wantsToMate = false;
            Needs[HORNEY] = 0f;
            JustBorn = false;
        }

        currentAge = currentAge + Time.deltaTime;
        SearchTime = SearchTime + Time.deltaTime;

        reproductiveUrge = reproductiveUrge + ((Time.deltaTime) * 0.2f);
        reproductiveUrge = UnityEngine.Mathf.Clamp(reproductiveUrge, MinReproductiveUrge, MaxReproductiveUrge);

        AgentDecisionMaking();
        ExecuteDecision();

        if (food <= 3f || energy <= 3f)
        {
            Destroy(gameObject);
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().GodAngelsDied += 1;
        }
        if (currentAge >= MaxAge)
        {
            Destroy(gameObject);
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().GodAngelsDied += 1;
        }
    }

    private void OnTriggerStay(Collider _collider)
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

    private void OnTriggerExit(Collider _collider)
    {
        if ((_collider.tag == "Work"))
        {
            working = false;
        }
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
        Collider[] _objectColliders = Physics.OverlapSphere(_agentPosition, _searchRadius, LayerMask.GetMask(_layerMask));

        foreach (Collider _object in _objectColliders) //Loop over the given object found
        {
            Vector3 objectPosition = _object.transform.position; //get object position

            // find distance to object
            Vector3 rawDistanceToObject = transform.position - objectPosition;
            float distanceToObject = Math.Abs(rawDistanceToObject.magnitude);

            //check if distance is smaller the the closest one yet
            if (distanceToObject < closestObjectDistance)
            {
                if (_object.gameObject.name != this.gameObject.name)
                {
                    _closestObject = _object.gameObject; //current object is closest else continue
                    closestObjectDistance = distanceToObject;
                }
                if (_object.gameObject.name == this.gameObject.name)
                {
                    _closestObject = null;
                }
            }
        }

        return _closestObject;
    }

    void MoveTo(Vector3 _target)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(_target);
        //float _velocityMag;
        //_velocityMag = navMeshAgent.velocity.magnitude;
        energy = energy - (Time.deltaTime * SpeedCost);
        food = food - (Time.deltaTime * SpeedCost);
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

    Vector3 RandomLocation(float _minX, float _maxX, float _minZ, float _maxZ)
    {
        Vector3 _location = new Vector3(UnityEngine.Random.Range(_minX, _maxX), 0, UnityEngine.Random.Range(_minZ, _maxZ));

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
            RemapedSearchTime = SearchTime;
            RemapedSearchTime = Remap(SearchTime, 0f, MaxSearchTime, 0f, 1f) * 100f;
            Needs[HUNGRY] = foodToHunger.Evaluate(food) * searchTimeToHunger.Evaluate(RemapedSearchTime);
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
        distToPoint = Math.Abs(Vector3.Distance(transform.position, SearchPoint));
        if (distToPoint < 2f)
        {
            searching = false;
        }


        // If need is Work, Go To Work
        if (mostUrgentNeedIndex == WORK)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.green;

            wantsToMate = false;
            eating = false;
            sleeping = false;

            if (working == true)
            {
                navMeshAgent.SetDestination(transform.position);
            }

            // if agent is not working
            if (working == false)
            {
                closestWork = FindClosestObject(transform.position, SearchRadius, "Work"); //find Closest Workplace

                // found close workplace //
                if (closestWork != null)
                {
                    //move to workplace
                    MoveTo(closestWork.transform.position);
                    searching = false;
                }

                // Has not found close work place //
                if (closestWork == null)
                {
                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        //get random location
                        Vector3 _location = RandomLocation(-SearchRadius * 2, SearchRadius * 2, -SearchRadius * 2, SearchRadius * 2);
                        _location = transform.position + _location;
                        //Check if location is valid
                        bool PositionValid = Physics.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                        if (PositionValid)
                        {
                            SearchPoint = _location;
                            searching = true;
                            MoveTo(SearchPoint);
                        }

                        if (PositionValid == false)
                        {
                            searching = false;
                        }
                    }

                    // if you are searching continue search
                    if (searching == true)
                    {
                        MoveTo(SearchPoint);
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

            if (eating == true)
            {
                navMeshAgent.SetDestination(transform.position);
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
                    SearchTime = 0f;
                    //move to Food
                    MoveTo(closestFood.transform.position);
                    searching = false;
                }

                // Has not found close Food //
                if (closestFood == null)
                {
                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        //get random location
                        Vector3 _location = RandomLocation(-SearchRadius * 2, SearchRadius * 2, -SearchRadius * 2, SearchRadius * 2);
                        _location = transform.position + _location;
                        //Check if location is valid
                        bool PositionValid = Physics.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                        if (PositionValid)
                        {
                            SearchPoint = _location;
                            searching = true;
                            MoveTo(SearchPoint);
                        }

                        if (PositionValid == false)
                        {
                            searching = false;
                        }
                    }

                    // if you are searching continue search
                    if (searching == true)
                    {
                        MoveTo(SearchPoint);
                    }
                }

            }

        }

        if (mostUrgentNeedIndex == TIRED)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.cyan;

            wantsToMate = false;
            working = false;
            eating = false;
            if (searching == false)
            {
                //get random location
                Vector3 _location = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius);
                _location = transform.position + _location;
                //Check if location is valid
                bool PositionValid = Physics.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                if (PositionValid)
                {
                    SearchPoint = _location;
                    searching = true;
                    MoveTo(SearchPoint);
                }

                if (PositionValid == false)
                {
                    searching = false;
                }
            }

            if (searching == true)
            {
                if (!navMeshAgent.pathPending)
                {
                    if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                    {
                        if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                        {
                            navMeshAgent.SetDestination(transform.position);
                            Sleep();
                        }
                    }
                }
            }
        }

        if (mostUrgentNeedIndex == HORNEY)
        {
            SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _renderer.material.color = Color.magenta;

            wantsToMate = true;
            working = false;
            eating = false;
            sleeping = false;

            if (foundMate)
            {
                if (closestMate != null)
                {
                    MoveTo(closestMate.transform.position);
                }
                if (closestMate == null)
                {
                    foundMate = false;
                }
            }

            if (foundMate == false) // dont have mate
            {
                closestMate = FindClosestObject(transform.position, SearchRadius, "Agent"); // find close agents

                // found agent
                if (closestMate != null)
                {
                    //check if he wants to mate
                    if (closestMate.transform.GetComponent<Agent>().wantsToMate)
                    {
                        MoveTo(closestMate.transform.position);
                        searching = false;
                        foundMate = true;
                    }
                    if (closestMate.transform.GetComponent<Agent>().wantsToMate == false)
                    {
                        Vector3 _location = RandomLocation(-SearchRadius * 2, SearchRadius * 2, -SearchRadius * 2, SearchRadius * 2);
                        _location = transform.position + _location;
                        bool PositionValid = Physics.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                        if (PositionValid)
                        {
                            SearchPoint = _location;
                            searching = true;
                            MoveTo(SearchPoint);
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
                        Vector3 _location = RandomLocation(-SearchRadius * 2, SearchRadius * 2, -SearchRadius * 2, SearchRadius * 2);
                        _location = transform.position + _location;
                        bool PositionValid = Physics.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));

                        if (PositionValid)
                        {
                            SearchPoint = _location;
                            searching = true;
                            MoveTo(SearchPoint);
                        }
                        if (PositionValid == false)
                        {
                            searching = false;
                        }
                    }

                    if (searching == true)
                    {
                        MoveTo(SearchPoint);
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

