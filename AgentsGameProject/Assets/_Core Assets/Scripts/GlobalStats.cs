using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalStats : MonoBehaviour
{

    public float RunTime = 0;
    public int Population = 0;
    public int AgentsBorn = 0;
    public int AgentsDied = 0;
    public float[] Stats;

    public float AvrageSpeed;
    public float AvrageSpeedCost;
    public float AvrageSearchRadius;
    public float AvrageWorkFoodCost;

    public float GodAngelsPopulation;
    public float GodAngelsDied;
    public float GodAngelsCreated;

    public float GodForce;

    public bool rotating = false;
    public bool moving = true;


    float startTime;
    public float TimerInterval = 1f;

    float updateTimer = 2f;
    Environment environment;
    //public GameObject NumOfAgentsVal;
    

    // Start is called before the first frame update
    void Awake()
    {
        Stats = new float[14];
        startTime = Time.time;
        environment = GameObject.Find("Environment").GetComponent<Environment>();
    }

    // Update is called once per frame
    void Update()
    {

        RunTime = Time.time - startTime;
        updateTimer = updateTimer + Time.deltaTime;

        if(updateTimer >= TimerInterval)
        {
            updateTimer = 0f;
            //GetStats
            Collider2D[] _agentsColliders = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("Agent"));
            
            float[] _agentsSpeeds;
            _agentsSpeeds = new float [_agentsColliders.Length];
            float[] _agentsSR;
            _agentsSR = new float[_agentsColliders.Length];
            float[] _agentsWorkCosts;
            _agentsWorkCosts = new float[_agentsColliders.Length];
            float[] _agentsSpeedCosts;
            _agentsSpeedCosts = new float[_agentsColliders.Length];

            float speedSum = 0f;
            float searchRadiusSum = 0f;
            float workFoodSum = 0f;
            float speedCostSum = 0f;

            for (int i = 0; i < _agentsColliders.Length; i++)
            {
                _agentsSpeeds[i] = _agentsColliders[i].transform.GetComponent<Agent>().AgentSpeed;
                speedSum = speedSum + _agentsSpeeds[i];
                _agentsSR[i] = _agentsColliders[i].transform.GetComponent<Agent>().SearchRadius;
                searchRadiusSum = searchRadiusSum + _agentsSR[i];
                _agentsWorkCosts[i] = _agentsColliders[i].transform.GetComponent<Agent>().WorkFoodCost;
                workFoodSum = workFoodSum + _agentsWorkCosts[i];
                _agentsSpeedCosts[i] = _agentsColliders[i].transform.GetComponent<Agent>().SpeedCost;
                speedCostSum = speedCostSum + _agentsSpeedCosts[i];
            }

            Population = AgentsBorn - AgentsDied + 2;

            AvrageSearchRadius = searchRadiusSum / _agentsSR.Length;
            AvrageSpeed = speedSum / _agentsSpeeds.Length;
            AvrageWorkFoodCost = workFoodSum / _agentsWorkCosts.Length;
            AvrageSpeedCost = speedCostSum / _agentsSpeedCosts.Length;


            GodAngelsPopulation = GodAngelsCreated - GodAngelsDied;

            Stats[0] = Population;
            Stats[1] = AgentsBorn;
            Stats[2] = AgentsDied;
            Stats[3] = AvrageSpeed;
            Stats[4] = AvrageSearchRadius;
            //Stats[5] = RunTime;
            Stats[6] = GodAngelsPopulation;
            Stats[7] = GodAngelsDied;
            Stats[8] = GodAngelsCreated;
            Stats[9] = AvrageWorkFoodCost;
            Stats[10] = AvrageSpeedCost;
        }
        Stats[11] = GodForce;
        Stats[12] = environment.Temperature;
        Stats[13] = environment.HeatEfficiency;
        Stats[5] = RunTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            moving = !moving;
            rotating = !rotating;
        }
    }
}
