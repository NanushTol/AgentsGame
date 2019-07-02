﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Pathfinding;

public class Agent : MonoBehaviour
{

    public float distToPoint;

    GameObject closestFood = null;
    GameObject closestWork = null;
    public GameObject closestMate = null;

    AIPath AstarAiPath;
    AIDestinationSetter aiDestinationSetter;
    GameObject DestinationTarget;
    public GameObject DestinationTargetPrefab;

    public GameObject AgentPrefab;


    public Dictionary<string, float> TraitsDic = new Dictionary<string, float>();
    public float[] Traits;
    public bool JustBorn;
    public bool JustBornWithAngel;

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
    
    
    public bool wantsToMate = false;
    public bool foundMate = false;
    public bool working;

    public float HasArrivedThresh = 3f;

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
    public float GfPerBirth;
    public float GeaneAvrage;
    public Vector3 _birthPosition;


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

    // Start is called before the first frame update
    void Awake()
    {
        SpriteRenderer _renderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        _renderer.material.color = new Color(0.962f, 0.276f, 0.448f, 1f);

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

        if (JustBornWithAngel == true)
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
            food = 20f;

            FoodSearchTime = 0f;
            currentAge = 0f;
            reproductiveUrge = 0f;
            wantsToMate = false;
            Needs[HORNEY] = 0f;
            JustBornWithAngel = false;
        }
        if (JustBorn == true)
        {
            //MaxAge = Traits[0];
            //MaxEnergy = Traits[1];
            //MaxFood = Traits[2];
            //MaxReproductiveUrge = Traits[3];
            SpeedCost = Traits[4];
            WorkFoodCost = Traits[5];
            //WorkEnergyCost = Traits[6];
            AgentSpeed = Traits[7];
            SearchRadius = Traits[8];
            energy = 100f;
            food = 20f;

            FoodSearchTime = 0f;
            currentAge = 0f;
            reproductiveUrge = 0f;
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

        if (food <= 3f || energy <= 3f)
        {
            Destroy(gameObject);
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
        }
        if (currentAge >= MaxAge)
        {
            Destroy(gameObject);
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
        }
    }

    private void OnTriggerStay2D(Collider2D _collider)
    {
        if((_collider.tag == "Work") && (mostUrgentNeedIndex == WORK))
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
            if(_collider.transform.gameObject != closestFood)
            {
                eating = false;
            }
            if (food >= FoodFullThreshold)
            {
                eating = false;
            }
        }
        if ((_collider.tag == "Agent") && (mostUrgentNeedIndex == HORNEY))
        {
            if(_collider.GetComponent<Agent>().wantsToMate == true)
            {
                if (energy > _collider.transform.GetComponent<Agent>().energy)
                {
                    Reproduce(_collider.transform.gameObject);
                }
            }  
        }
        if ((_collider.tag == "GodAngel") && (mostUrgentNeedIndex == HORNEY))
        {
            if (_collider.GetComponent<GodAngel>().wantsToMate == true)
            {
                    ReproduceGodAngel(_collider.transform.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _collider)
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
        Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(_agentPosition, _searchRadius, LayerMask.GetMask(_layerMask)); 

        foreach (Collider2D _object in _objectColliders) //Loop over the given object found
        {
            Vector3 objectPosition = _object.transform.position; //get object position
            Vector3 thisPosition = gameObject.transform.position;
            // find distance to object
            //Vector3 rawDistanceToObject = transform.position - objectPosition;
            float distanceToObject = Vector3.Distance(objectPosition, thisPosition);
            
            //check if distance is smaller the the closest one yet
            if(distanceToObject < closestObjectDistance)
            {
                if(_object.gameObject.name != this.gameObject.name)
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

    public void Reproduce(GameObject _mate)
    {
        // reset reproductive urges

            _mate.GetComponent<Agent>().wantsToMate = false;
            _mate.GetComponent<Agent>().foundMate = false;
            _mate.GetComponent<Agent>().horney = 1f;
            _mate.GetComponent<Agent>().reproductiveUrge = 0.0f;

        wantsToMate = false;
        foundMate = false;
        horney = 1f;
        reproductiveUrge = 0.0f;

        GameObject _globalStats = GameObject.Find("GlobalStats");
        _globalStats.GetComponent<GlobalStats>().AgentsBorn += 1;
        _globalStats.GetComponent<GlobalStats>().GodForce += GfPerBirth;

        //Vector3 _birthPosition = new Vector3(0, 0, 2);
        //get random location
        for(int j = 0; j < 20; j++)
        {
            _birthPosition = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius) + gameObject.transform.position;

            _birthPosition.z = 0.0f;

            //Check if location is valid
            Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(transform.position, 0.4f, LayerMask.GetMask("Agent")); //check agent on location
            bool PositionValid = Physics2D.Raycast(_birthPosition, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground")); //check on ground

            if (PositionValid && _objectColliders.Length == 0)
            {
                j = 21;
            }
        }
            
            //bool PositionValid = Physics.Raycast(_birthPosition, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));

        Quaternion _rotation = new Quaternion(0, 0, 0, 0);
        GameObject _baby = Instantiate(AgentPrefab, _birthPosition, _rotation); // create a baby as a child object
        _baby.name = "Agent (clone) " + UnityEngine.Random.Range(1000, 1999);
        _baby.GetComponent<Agent>().JustBorn = true;
        
        float[] _babyTraits = _baby.GetComponent<Agent>().Traits; // get baby traits

        _baby.GetComponent<Agent>().MissionInLife = MissionInLife; //set mission in life to self mission

        // Deside which perent's trait to take by chance
        float _randomHeritage = UnityEngine.Random.Range(0, 1); //get chance
        
        /*
        for (int i = 0; i < _babyTraits.Length; i++)
        {
            //_babyTraits[i] = ((Traits[i] * (UnityEngine.Random.Range(-Mutaion, Mutaion))) + Traits[i]);
            GeaneAvrage = (_mate.GetComponent<Agent>().Traits[i] + Traits[i]) / 2;
            _babyTraits[i] = (GeaneAvrage * (UnityEngine.Random.Range(-Mutaion, Mutaion))) + GeaneAvrage;
        }*/

        // ((Traits[i] * (UnityEngine.Random.Range(-Mutaion, Mutaion))) + Traits[i]);
        //((_mate.GetComponent<Agent>().Traits[i] * (UnityEngine.Random.Range(-Mutaion, Mutaion))) + Traits[i]);
        
        for (int i = 0; i < _babyTraits.Length; i++)
        {
            //var _element = _babyTraits.ElementAt(i);
            if (_randomHeritage <= 0.50) // baby get self trait
            {
                _babyTraits[i] = ((Traits[i] * (UnityEngine.Random.Range(-Mutaion, Mutaion))) + Traits[i]);
            }
            if (_randomHeritage > 0.50) // baby get mate trait
            {
                _babyTraits[i] = ((_mate.GetComponent<Agent>().Traits[i] * (UnityEngine.Random.Range(-Mutaion, Mutaion))) + _mate.GetComponent<Agent>().Traits[i]);
            }
        }
    }

    public void ReproduceGodAngel(GameObject _mate)
    {
        // reset reproductive urges

            _mate.GetComponent<GodAngel>().wantsToMate = false;
            _mate.GetComponent<GodAngel>().foundMate = false;
            _mate.GetComponent<GodAngel>().horney = 1f;
            _mate.GetComponent<GodAngel>().reproductiveUrge = 0.0f;

        wantsToMate = false;
        foundMate = false;
        horney = 1f;
        reproductiveUrge = 0.0f;

        GameObject _globalStats = GameObject.Find("GlobalStats");
        _globalStats.GetComponent<GlobalStats>().AgentsBorn += 1;
        _globalStats.GetComponent<GlobalStats>().GodForce += GfPerBirth;

        for (int j = 0; j < 20; j++)
        {
            _birthPosition = (UnityEngine.Random.insideUnitSphere * SearchRadius) + gameObject.transform.position;
            _birthPosition.y = 0.3f;
            //Check if location is valid
            RaycastHit _hit;
            bool PositionValid = Physics.SphereCast(_birthPosition, 1.3f, Camera.main.transform.forward, out _hit, 100f, LayerMask.GetMask("Agent"));
            if (PositionValid == false)
            {
                j = 21;
            }
        }

        Quaternion _rotation = new Quaternion(0, 0, 0, 0);
        GameObject _baby = Instantiate(AgentPrefab, (transform.position + _birthPosition), _rotation); // create a baby as a child object
        _baby.name = "Agent (clone) " + UnityEngine.Random.Range(1000, 1999);
        _baby.GetComponent<Agent>().JustBornWithAngel = true;

        float[] _babyTraits = _baby.GetComponent<Agent>().Traits; // get baby traits

        _baby.GetComponent<Agent>().MissionInLife = MissionInLife; //set mission in life to self mission

        _baby.GetComponent<Agent>().AgentSpeed = (_mate.GetComponent<GodAngel>().AgentSpeed + AgentSpeed) / 2;
        _baby.GetComponent<Agent>().SearchRadius = (_mate.GetComponent<GodAngel>().SearchRadius + SearchRadius) / 2;
        _baby.GetComponent<Agent>().SpeedCost = (_mate.GetComponent<GodAngel>().SpeedCost + SpeedCost) / 2;
        _baby.GetComponent<Agent>().WorkFoodCost = (_mate.GetComponent<GodAngel>().WorkFoodCost + WorkFoodCost) / 2;

        //for (int i = 0; i < _babyTraits.Length; i++)
        //{
        //        _babyTraits[i] = ((Traits[i] + _mate.GetComponent<GodAngel>().Traits[i]) / 2);
        //}
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
                if (closestWork != null)
                {
                    //move to workplace
                    MoveTo(closestWork);
                    searching = false;
                }

                // Has not found close work place //
                if (closestWork == null)
                {
                    //if You are not searching find a new searchPoint
                    if (searching == false)
                    {
                        //get random location
                        Vector3 _location = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius);
                        _location += transform.position;

                        //Check if location is valid
                        bool PositionValid = Physics2D.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
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
                        bool PositionValid = Physics2D.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
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

            if(searching == false && sleeping == false)
            {
                //get random location
                Vector3 _location = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius);
                _location = transform.position + _location;
                //Check if location is valid
                bool PositionValid = Physics2D.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
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
                if(closestMate != null)
                {
                    searching = false;
                    MoveTo(closestMate);
                }
                if(closestMate == null)
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
                        bool PositionValid = Physics2D.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
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
                        bool PositionValid = Physics2D.Raycast(_location, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));

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

