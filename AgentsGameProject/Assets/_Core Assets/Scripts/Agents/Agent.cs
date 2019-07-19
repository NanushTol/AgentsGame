using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Pathfinding;

public class Agent : MonoBehaviour
{
    public GameObject AgentPrefab;

    #region // Agent's States
    [Header("States")]
    public bool JustBorn;
    public bool JustBornWithAngel;
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

    public string mostUrgentNeed = ("null");
    [HideInInspector]
    public int mostUrgentNeedIndex;

    public float readyForWork = 0.0f;
    public float hungry = 0.0f;
    public float tierd = 0.0f;
    public float horney = 0.0f;

    public float currentAge;
    public float food = 1f;
    public float energy = 1f;
    public float reproductiveMultiplier = 0f;
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


    #region //Shared Parameters
    [Header("Shared Parameters")]

    public float ConsumptionScale = 0.1f;
    public float MaxEnergy = 100;
    public float MaxFood = 100;
    public float MaxReproductiveMultiplier = 1f;
    public float MinReproductiveUrgeMultiplier = 0f;
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

    #endregion


    #region // General Variables

    float timer = 0f;
    Vector3 SearchPoint;
    Vector3 _birthPosition;
    Vector3 enterPosition;

    GameObject closestFood = null;
    GameObject closestWork = null;
    GameObject closestMate = null;

    GameObject globalStats;

    ResourcesDataController resourcesDataController;

    #endregion


    #region // AStar Variables
    AIPath AstarAiPath;
    AIDestinationSetter aiDestinationSetter;
    GameObject DestinationTarget;
    public GameObject DestinationTargetPrefab;
    #endregion


    #region // Needs
    private float[] Needs;
    private const int HUNGRY = 0;
    private const int TIRED = 1;
    private const int WORK = 2;
    private const int HORNEY = 3;
    #endregion


    #region // Fuzzy Logic Graphs

    [Header("Hunger Graphs")]
    public AnimationCurve foodToHunger;
    public AnimationCurve searchTimeToHunger;

    [Header("Energy Graphs")]
    public AnimationCurve energyToTiredness;
    public AnimationCurve energyToReadyness;
    public AnimationCurve searchTimeToReadyness;

    [Header("Horny Graphs")]
    public AnimationCurve ageToHorney;

    #endregion


    //public Dictionary<string, float> TraitsDic = new Dictionary<string, float>();
    [HideInInspector]
    public float[] Traits;

    // Start is called before the first frame update
    void Awake()
    {
        SpriteRenderer _renderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        _renderer.color = AgentColor;
        //new Color(0.962f, 0.276f, 0.448f, 1f);
        //F54772

        resourcesDataController = GameObject.Find("GameManager").GetComponent<ResourcesDataController>();

        Traits = new float[6];

        Traits[0] = MaxAge;
        Traits[1] = ReproductiveUrge;
        Traits[2] = FoodConsumption;
        Traits[3] = EnergyConsumption;
        Traits[4] = WorkingSpeed;
        Traits[5] = Size;

        Needs = new float[4];


        #region // Pathfinding
        GameObject targetParent = GameObject.Find("AgentsTargets");
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        DestinationTarget = Instantiate(DestinationTargetPrefab, targetParent.transform);
        DestinationTarget.name = gameObject.name + ("Target");
        DestinationTarget.transform.parent = targetParent.transform;
        AstarAiPath = gameObject.GetComponent(typeof(AIPath)) as AIPath;
        AstarAiPath.maxSpeed = AgentSpeed;
        #endregion

        globalStats = GameObject.Find("GlobalStats");
    }

       // Update is called once per frame
    void Update()
    {

        if (JustBorn == true)
        {
            MaxAge = Traits[0];
            ReproductiveUrge = Traits[1];
            FoodConsumption = Traits[2];
            EnergyConsumption = Traits[3];
            WorkingSpeed = Traits[4];
            Size = Traits[5];

            SpriteRenderer _renderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
            _renderer.color = AgentColor;

            energy = 90f;
            food = 20f;

            FoodSearchTime = 0f;
            currentAge = 0f;
            reproductiveMultiplier = 0f;
            wantsToMate = false;
            Needs[HORNEY] = 0f;
            JustBorn = false;
        }
        

        AgentDecisionMaking();
        ExecuteDecision();
    }

    void LateUpdate()
    {
        currentAge = currentAge + Time.deltaTime;
        
        

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
            reproductiveMultiplier = reproductiveMultiplier * 0f;
            FoodSearchTime += Time.deltaTime;
        }
        if (WorkSearchTime > MaxSearchTime - (MaxSearchTime * 0.15f))
        {
            WorkSearchTime += Time.deltaTime;
        }

        globalStats.GetComponent<GlobalStats>().GodForce += 0.05f * Time.deltaTime;

        reproductiveMultiplier = reproductiveMultiplier + ((Time.deltaTime) * 0.1f);
        reproductiveMultiplier = UnityEngine.Mathf.Clamp(reproductiveMultiplier, MinReproductiveUrgeMultiplier, MaxReproductiveMultiplier);

        if (food <= 3f)
        {
            if(InBuilding) closestWork.GetComponent<GenericBuilding>().AgentsWorking -= 1;

            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Hunger");
            Destroy(gameObject);
        }
        if (energy <= 3f)
        {
            if (InBuilding) closestWork.GetComponent<GenericBuilding>().AgentsWorking -= 1;
            
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Exhaustion");
            Destroy(gameObject);
        }
        if (currentAge >= MaxAge)
        {
            if (InBuilding) closestWork.GetComponent<GenericBuilding>().AgentsWorking -= 1;
            
            GameObject _globalStats = GameObject.Find("GlobalStats");
            _globalStats.GetComponent<GlobalStats>().AgentsDied += 1;
            Debug.Log("Agent Died of Old Age");
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D _collider)
    {
        if((_collider.tag == "Work") && (mostUrgentNeedIndex == WORK))
        {
            if (closestWork.GetComponent<GenericBuilding>().WorkersNeeded)
            {
                EnterBuilding();
                working = true;
            }
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
                if (currentAge + energy + name.Length > _collider.transform.GetComponent<Agent>().currentAge + _collider.transform.GetComponent<Agent>().energy + _collider.transform.GetComponent<Agent>().name.Length)
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

    GameObject FindClosestMate(Vector3 _agentPosition, float _searchRadius)
    {
        int agent = 1 << LayerMask.NameToLayer("Agent");
        int godAngel = 1 << LayerMask.NameToLayer("GodAngel");
        int mask = agent | godAngel;

        float closestObjectDistance = 1000f;
        GameObject _closestObject = null;

        //get all object in a given radius
        Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(_agentPosition, _searchRadius, mask);

        // no others found
        if (_objectColliders.Length <= 0)
        {
            _closestObject = null;
        } 

        // found others
        else if(_objectColliders.Length > 0)
        {
            foreach (Collider2D _object in _objectColliders) //Loop over the given object found
            {
                if (_object.gameObject.name != this.gameObject.name)
                {
                    //get objects position
                    Vector3 objectPosition = _object.transform.position;
                    Vector3 thisPosition = gameObject.transform.position;

                    // find distance to object
                    float distanceToObject = Vector3.Distance(objectPosition, thisPosition);

                    if(_object.tag == "Agent")
                    {
                        if (_object.GetComponent<Agent>().wantsToMate)
                        {
                            //check if distance is smaller the the closest one yet
                            if (distanceToObject < closestObjectDistance)
                            {
                                _closestObject = _object.gameObject;
                                closestObjectDistance = distanceToObject;
                            }
                        }
                    }
                    else if (_object.tag == "GodAngel")
                    {
                        if (_object.GetComponent<GodAngel>().wantsToMate)
                        {
                            //check if distance is smaller the the closest one yet
                            if (distanceToObject < closestObjectDistance)
                            {
                                _closestObject = _object.gameObject;
                                closestObjectDistance = distanceToObject;
                            }
                        }
                    }
                    
                    else
                    {
                        _closestObject = null;
                    } 
                }
            }
        }

        return _closestObject;
    }

    public void Reproduce(GameObject _mate)
    {
        // reset this & partner reproductive urges

        _mate.GetComponent<Agent>().wantsToMate = false;
        _mate.GetComponent<Agent>().foundMate = false;
        _mate.GetComponent<Agent>().horney = 1f;
        _mate.GetComponent<Agent>().reproductiveMultiplier = 0.0f;

        wantsToMate = false;
        foundMate = false;
        horney = 1f;
        reproductiveMultiplier = 0.0f;

        globalStats.GetComponent<GlobalStats>().AgentsBorn += 1;
        globalStats.GetComponent<GlobalStats>().GodForce += GfPerBirth;

        #region // Create Baby
        //get random location
        for (int j = 0; j < 30; j++)
        {
            _birthPosition = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius) + gameObject.transform.position;

            _birthPosition.z = 0.0f;

            //Check if location is valid
            Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(_birthPosition, 0.4f, LayerMask.GetMask("Agent")); //check agent on location

            bool PositionValid = Physics2D.CircleCast(_birthPosition, 0.55f, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));//check on ground

            if (PositionValid && _objectColliders.Length == 0)
            {
                j = 31;
            }
        }



        //Instantiate Baby
        Quaternion _rotation = new Quaternion(0, 0, 0, 0);
        GameObject _baby = Instantiate(AgentPrefab, _birthPosition, _rotation); // create a baby as a child object
        _baby.name = "Agent (clone) " + UnityEngine.Random.Range(1000, 1999);
        _baby.GetComponent<Agent>().JustBorn = true;

        #endregion


        #region // Set Baby's Traits
        float[] _babyTraits = _baby.GetComponent<Agent>().Traits; // get baby traits



        _baby.GetComponent<Agent>().AgentType = AgentType; //set mission in life to self mission



        // Deside which perent's trait to take by chance
        float _randomHeritage = UnityEngine.Random.Range(0, 1); //get chance
        
        //Loop Traits & mutate
        for (int i = 0; i < _babyTraits.Length; i++)
        {

            if (_randomHeritage <= 0.50) // baby get self trait
            {
                _babyTraits[i] = ((Traits[i] * (UnityEngine.Random.Range(-Mutaion, Mutaion))) + Traits[i]);
            }
            else if (_randomHeritage > 0.50) // baby get mate trait
            {
                _babyTraits[i] = ((_mate.GetComponent<Agent>().Traits[i] * (UnityEngine.Random.Range(-Mutaion, Mutaion))) + _mate.GetComponent<Agent>().Traits[i]);
            }

            if(i > 0) // dont clamp max life
            {
                _babyTraits[i] = Mathf.Clamp(_babyTraits[i], 1f, 10f); // limit trait from 1 to 10
            }
        }

        #endregion


        #region // Baby Color Settings
        // baby Color
        float h, s, v;
        float mateH, mateS, mateV;

        //Conver self Color To HSV
        Color.RGBToHSV(AgentColor, out h, out s, out v);

        // Conver Partners Color to HSV
        Color.RGBToHSV(_mate.GetComponent<Agent>().AgentColor, out mateH, out mateS, out mateV);


        // Average Colors
        h = (h + mateH) / 2f;

        // Mutate Color by 5%
        h += (h * UnityEngine.Random.Range(-0.05f, 0.05f));

        // clamp to valid values
        h = Mathf.Clamp(h, 0f, 360f);

        // Assign Color to baby
        Color babyColor = Color.HSVToRGB(h, s, v);
        _baby.GetComponent<Agent>().AgentColor = babyColor;
        #endregion
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
        reproductiveMultiplier = 0.0f;

        GameObject _globalStats = GameObject.Find("GlobalStats");
        _globalStats.GetComponent<GlobalStats>().AgentsBorn += 1;
        _globalStats.GetComponent<GlobalStats>().GodForce += GfPerBirth;

        //get random location
        for (int j = 0; j < 30; j++)
        {
            _birthPosition = RandomLocation(-SearchRadius, SearchRadius, -SearchRadius, SearchRadius) + gameObject.transform.position;

            _birthPosition.z = 0.0f;

            //Check if location is valid
            Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(_birthPosition, 0.4f, LayerMask.GetMask("Agent")); //check agent on location

            bool PositionValid = Physics2D.CircleCast(_birthPosition, 0.55f, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));//check on ground

            if (PositionValid && _objectColliders.Length == 0)
            {
                j = 31;
            }
        }

        Quaternion _rotation = new Quaternion(0, 0, 0, 0);
        GameObject _baby = Instantiate(AgentPrefab, _birthPosition, _rotation); // create a baby as a child object
        _baby.name = "Agent (clone) " + UnityEngine.Random.Range(1000, 1999);
        _baby.GetComponent<Agent>().JustBornWithAngel = true;

        float[] _babyTraits = _baby.GetComponent<Agent>().Traits; // get baby traits

        _baby.GetComponent<Agent>().AgentType = AgentType; //set mission in life to self mission

        _baby.GetComponent<Agent>().AgentSpeed = (_mate.GetComponent<GodAngel>().AgentSpeed + AgentSpeed) / 2f;
        _baby.GetComponent<Agent>().SearchRadius = (_mate.GetComponent<GodAngel>().SearchRadius + SearchRadius) / 2f;
        _baby.GetComponent<Agent>().SpeedCost = (_mate.GetComponent<GodAngel>().SpeedCost + SpeedCost) / 2f;
        _baby.GetComponent<Agent>().WorkFoodCost = (_mate.GetComponent<GodAngel>().WorkFoodCost + WorkFoodCost) / 2f;

        _baby.GetComponent<Agent>().AgentColor = _mate.GetComponent<GodAngel>().GodAngelColor;
        //_baby.GetComponent<Agent>().Color = ;

        //for (int i = 0; i < _babyTraits.Length; i++)
        //{
        //        _babyTraits[i] = ((Traits[i] + _mate.GetComponent<GodAngel>().Traits[i]) / 2);
        //}
    }

    void MoveTo(GameObject _target)
    {
        aiDestinationSetter.target = _target.transform;

        float _velocity = GetComponent<Rigidbody2D>().velocity.magnitude;

        energy = energy - (_velocity * EnergyConsumption * Size * Time.deltaTime * ConsumptionScale);
        food = food - (_velocity * FoodConsumption * Size * Time.deltaTime * ConsumptionScale);
    }

    void Work(float _workFoodCost, float _workEnergyCost, float _workingSpeed, float _size, GameObject _closestWork)
    {
        working = true;
        float _workPlaceEfficiency = _closestWork.GetComponent<GenericBuilding>().WorkEfficiency;
        float _workPlaceProduction = _closestWork.GetComponent<GenericBuilding>().Production;

        

        //create Production
        _workPlaceProduction += _workPlaceEfficiency * _workingSpeed * Time.deltaTime;
        _closestWork.GetComponent<GenericBuilding>().Production = _workPlaceProduction;

        //reduce Energy & Food
        food = food - (_size * _workFoodCost * Time.deltaTime * ConsumptionScale);
        energy = energy - (_size * _workEnergyCost * Time.deltaTime * ConsumptionScale);
    }

    void Eat(GameObject _food)
    {
        eating = true;
        float _bite;

        SpriteRenderer _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _renderer.material.color = Color.yellow;

        //float _foodvalue = _food.GetComponent<Food>().FoodValue;
        _bite = (BiteSize * Time.deltaTime);

        food = food + _bite;

        _food.GetComponent<Food>().FoodValue -= _bite;

        resourcesDataController.UpdateResourceProduction("Food", -_bite);
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

    void EnterBuilding()
    {
        InBuilding = true;
        enterPosition = transform.position;
        closestWork.GetComponent<GenericBuilding>().AgentsWorking += 1;

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);

        GetComponent<AIPath>().enabled = false;
        GetComponent<AIDestinationSetter>().enabled = false;
        GetComponent<SimpleSmoothModifier>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
    }

    void ExitBuilding()
    {
        InBuilding = false;
        transform.position = enterPosition;
        closestWork.GetComponent<GenericBuilding>().AgentsWorking -= 1;

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(true);

        GetComponent<AIPath>().enabled = true;
        GetComponent<AIDestinationSetter>().enabled = true;
        GetComponent<SimpleSmoothModifier>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().simulated = true;

        aiDestinationSetter.target = transform;
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
            Needs[HUNGRY] = 1;
            hungry = foodToHunger.Evaluate(Remap(food, 0f, MaxFood, 0f, 1f)) - (searchTimeToHunger.Evaluate(RemapedFoodSearchTime));
        }
        if (eating == false)
        {

            RemapedFoodSearchTime = Remap(FoodSearchTime, 0f, MaxSearchTime, 0f, 1f);
            Needs[HUNGRY] = foodToHunger.Evaluate(Remap(food, 0f, MaxFood, 0f, 1f)) - (searchTimeToHunger.Evaluate(RemapedFoodSearchTime));
            hungry = Needs[HUNGRY];
        }



        if (sleeping)
        {
            Needs[TIRED] = 1;
            tierd = energyToTiredness.Evaluate((Remap(energy, 0f, MaxEnergy, 0f, 1f)));
        }
        if (sleeping == false)
        {
            Needs[TIRED] = energyToTiredness.Evaluate((Remap(energy, 0f, MaxEnergy, 0f, 1f)));
            tierd = Needs[TIRED];
        }



        RemapedWorkSearchTime = Remap(WorkSearchTime, 0f, MaxSearchTime, 0f, 1f);
        Needs[WORK] = energyToReadyness.Evaluate((Remap(energy, 0f, MaxEnergy, 0f, 1f))) - (searchTimeToReadyness.Evaluate(RemapedWorkSearchTime));
        readyForWork = Needs[WORK];



        Needs[HORNEY] = ageToHorney.Evaluate(Remap(currentAge, 0f, MaxAge, 0f, 1f)) * reproductiveMultiplier * (Remap(globalStats.GetComponent<GlobalStats>().Population, 0f, 150f, 1f, 0f));
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

        if(working && _mostUrgentNeedIndex != WORK)
        {
            ExitBuilding();
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
                //aiDestinationSetter.target = transform;
                Work(FoodConsumption, EnergyConsumption, WorkingSpeed, Size, closestWork);
            }

            // if agent is not working
            if (working == false)
            {
                closestWork = FindClosestObject(transform.position, SearchRadius, "Work"); //find Closest Workplace

                // found close workplace //
                if (closestWork != null && closestWork.GetComponent<GenericBuilding>().WorkersNeeded)
                {
                    WorkSearchTime = 0f;
                    //move to workplace
                    MoveTo(closestWork);
                    searching = false;
                }

                // Has not found close work place //
                if (closestWork == null || closestWork.GetComponent<GenericBuilding>().WorkersNeeded == false)
                {
                    WorkSearchTime = WorkSearchTime + Time.deltaTime;

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
            //workSearchTime = 0f;

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
                    FoodSearchTime = FoodSearchTime + Time.deltaTime;

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

            wantsToMate = false;
            working = false;
            eating = false;
            //FoodSearchTime = 0f;
            //workSearchTime = 0f;


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

            wantsToMate = true;
            working = false;
            eating = false;
            sleeping = false;
            hasArraived = false;
            //FoodSearchTime = 0f;
            //workSearchTime = 0f;


            if (foundMate)
            {
                if(closestMate != null)
                {
                    if (closestMate.tag == ("GodAngel"))
                    {
                        if (closestMate.GetComponent<GodAngel>().wantsToMate)
                        {
                            searching = false;
                            MoveTo(closestMate);
                        }
                        if (closestMate.GetComponent<GodAngel>().wantsToMate == false)
                        {
                            closestMate = null;
                        }
                    }

                    else if (closestMate.tag == ("Agent"))
                    {
                        if (closestMate.GetComponent<Agent>().wantsToMate)
                        {
                            searching = false;
                            MoveTo(closestMate);
                        }
                        if (closestMate.GetComponent<Agent>().wantsToMate == false)
                        {
                            closestMate = null;
                        }
                    } 
                }
                if(closestMate == null)
                {
                    foundMate = false;
                }
            }

            if (foundMate == false) // dont have mate
            {
                closestMate = FindClosestMate(transform.position, SearchRadius); // find closest mate

                // found agent
                if (closestMate != null) 
                {
                    if (closestMate.tag == ("GodAngel"))
                    {
                            searching = false;
                            MoveTo(closestMate);
                    }

                    else if (closestMate.tag == ("Agent"))
                    {

                            searching = false;
                            MoveTo(closestMate);
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

