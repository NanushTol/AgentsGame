using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public class WorkPlace : MonoBehaviour
{
    public Grid grid;

    public GameObject FoodPrefab;
    public float Production = 0.0f;

    public float GrowingRadius = 10f;
    public float WorkersRadius = 5f;

    public float WorkEfficiency;
    public int MaxWorkers = 6;
    public int CurrntlyWorking;

    public Color WorkingColor;
    public Color NotWorkingColor;

    public bool WorkersNeeded;

    [HideInInspector]
    public Vector3Int LastPosition;

    bool feedingFruit = false;

    GameObject foodSource = null;

    GameObject environment;

    float spherecastTimer = 0;

    int[] agentsWorking = new int[0];

    bool PositionValid;

    // Start is called before the first frame update
    void Awake()
    {
        environment = GameObject.Find("Environment");
        grid = GameObject.Find("Grid").GetComponent<Grid>();
    }
    private void Start()
    {
        Vector3Int position = grid.WorldToCell(transform.position);

        position.x += 18;
        position.y += 17;

        LastPosition = position;

        var gg = AstarPath.active.data.gridGraph;
        int x = position.x;
        int y = position.y;
        GridNodeBase node = gg.GetNode(x, y);

        AstarPath.active.AddWorkItem(ctx => {
            var PfGridGraph = AstarPath.active.data.gridGraph;

            // Mark a single node as unwalkable
            PfGridGraph.GetNode(x, y).Walkable = false;

            // Recalculate the connections for that node as well as its neighbours
            PfGridGraph.CalculateConnectionsForCellAndNeighbours(x, y);
        });

        //transform.GetChild(1).localScale = new Vector3(GrowingRadius * 2f, 0.1f, GrowingRadius * 2f);
    }

    // Update is called once per frame
    void Update()
    {
        spherecastTimer = spherecastTimer + Time.deltaTime;

        if (spherecastTimer >= 0.5)
        {
            Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(transform.position, WorkersRadius, LayerMask.GetMask("Agent"));



            CurrntlyWorking = 0;

            for(int a = 0; a < _objectColliders.Length; a++)
            {
                if (_objectColliders[a].GetComponent<Agent>().working)
                {
                    CurrntlyWorking += 1;
                }
            }

            agentsWorking = new int[CurrntlyWorking];

            if(agentsWorking.Length <= 6)
            {

                WorkersNeeded = true;

                if (agentsWorking.Length == 6)
                {
                    WorkersNeeded = false;
                }
                
            }
            if (agentsWorking.Length > 6)
            {
                agentsWorking = new int[6];
                WorkersNeeded = false;
            }


            spherecastTimer = 0;
        }

        UpdateVacancyBar(agentsWorking);

        if(Production > 0)
        {
            if (foodSource == null || feedingFruit == false) // if no fruit available create fruit
            {

                Vector3 foodPosition = new Vector3(UnityEngine.Random.Range(-GrowingRadius, GrowingRadius), 
                    UnityEngine.Random.Range(-GrowingRadius, GrowingRadius), 0f);
                Quaternion _rotation = new Quaternion(0, 0, 0, 0);

                for (int i = 0; i < 20; i++)
                {
                    PositionValid = Physics2D.Raycast(foodPosition + transform.position, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));
                    if (PositionValid)
                    {
                        i = 21;
                    }
                }

                if (PositionValid)
                {
                    foodSource = Instantiate(FoodPrefab, foodPosition + transform.position, _rotation);

                    Production = Production - (WorkEfficiency * Time.deltaTime * agentsWorking.Length);
                    foodSource.GetComponent<Food>().FoodValue += WorkEfficiency * Time.deltaTime * agentsWorking.Length;

                    PositionValid = false;
                }
            }

            if (foodSource != null) //If there is a fruit feed fruit up to 20(max fruit value)
            {
                if (foodSource.GetComponent<Food>().FoodValue < 20) // feed fruit
                {
                    foodSource.GetComponent<Food>().FoodValue += WorkEfficiency * Time.deltaTime * agentsWorking.Length;// * environment.GetComponent<Environment>().HeatEfficiency;

                    Production = Production - (WorkEfficiency * Time.deltaTime * agentsWorking.Length);// * environment.GetComponent<Environment>().HeatEfficiency);

                    //environment.GetComponent<Environment>().WorkTempIn += environment.GetComponent<Environment>().WorkTempCost * Time.deltaTime;

                    feedingFruit = true;
                }
                if (foodSource.GetComponent<Food>().FoodValue >= 20) // Finish feeding and relese fruit
                {
                    foodSource.tag = "Food";
                    foodSource.layer = 8;
                    feedingFruit = false;
                    foodSource = null;
                }
            }
        }
        

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GrowingRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, WorkersRadius);

    }

    public void UpdateNode(Vector3Int _position, bool _walkable)
    {
        AstarPath.active.AddWorkItem(ctx => {
            var PfGridGraph = AstarPath.active.data.gridGraph;

            // Mark a single node as unwalkable
            PfGridGraph.GetNode(_position.x, _position.y).Walkable = _walkable;

            // Recalculate the connections for that node as well as its neighbours
            PfGridGraph.CalculateConnectionsForCellAndNeighbours(_position.x, _position.y);
        });
    }

    void UpdateVacancyBar(int[] _agentsWorking)
    {
        Transform vacancyBar = transform.GetChild(0);

        for(int j = 0; j < MaxWorkers; j++)
        {
            vacancyBar.transform.GetChild(j).gameObject.GetComponent<SpriteRenderer>().color = NotWorkingColor;
        }

        for(int i = 0; i < _agentsWorking.Length; i++)
        {
            vacancyBar.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = WorkingColor;
        }
        
    }
}
