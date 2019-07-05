using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateAngel : MonoBehaviour
{
    #region //public variables
    public GameObject AngelPrefab;
    public TMP_InputField SearchRadiusField;
    public TMP_InputField SpeedField;
    public TMP_InputField SpeedCostField;
    public TMP_InputField WorkFoodCostField;
    public GameObject ColorButton;

    public GameObject globalStats;
    public float BaseGfCost;

    [HideInInspector]
    public float GfCostSearchRadius;
    [HideInInspector]
    public float GfCostAgentSpeed;
    [HideInInspector]
    public float GfCostSpeedCost;
    [HideInInspector]
    public float GfCostWorkFoodCost;
    [HideInInspector]
    public float[] GodForceCosts;

    [Header("Traits")]
    
    public string MissionInLife = "Angel";
    public float MaxAge = 60f;
    public float MaxEnergy = 100;
    public float MaxFood = 100;
    public float MaxReproductiveUrge = 1f;
    public float MinReproductiveUrge = 0f;
    public float SpeedCost = 0.3f;
    public float WorkFoodCost = 1.8f;
    public float WorkEnergyCost = 1.2f;
    public float AgentSpeed = 10f;
    public float SearchRadius = 8f;
    public float SleepEfficiency = 2f;
    public float FoodFullThreshold = 60f;
    public float AwakeThreshold = 70f;
    public float Mutaion = 0.15f;
    public Color GodAngelColor;
    #endregion

    #region //local variables
    float gfSum;

    GameObject newBornAngel;
    Vector3 birthPosition;
    Quaternion rotation = new Quaternion(0, 0, 0, 0);
    RaycastHit hit;
    float yOffset = 5f;
    Vector3 creationPoint;
    [HideInInspector]
    public bool creatingGodAngel;
    float distance;
    Vector3 v3;
    Grid grid;
    float lastTimeScale;

    #endregion
    private void Awake()
    {
        GodForceCosts = new float[5];
        grid = GameObject.Find("Grid").GetComponent<Grid>();


    }
    void Update()
    {
        //Get Input Traits
        SearchRadius = float.Parse(SearchRadiusField.GetComponent<TMP_InputField>().text);
        AgentSpeed = float.Parse(SpeedField.GetComponent<TMP_InputField>().text);
        SpeedCost = float.Parse(SpeedCostField.GetComponent<TMP_InputField>().text);
        WorkFoodCost = float.Parse(WorkFoodCostField.GetComponent<TMP_InputField>().text);
        GodAngelColor = ColorButton.GetComponent<Image>().color;

        GfCostSearchRadius = Mathf.Abs(SearchRadius - globalStats.GetComponent<GlobalStats>().AvrageSearchRadius);
        GfCostAgentSpeed = Mathf.Abs(AgentSpeed - globalStats.GetComponent<GlobalStats>().AvrageSpeed);
        GfCostSpeedCost = Mathf.Abs(SpeedCost - globalStats.GetComponent<GlobalStats>().AvrageSpeedCost);
        GfCostWorkFoodCost = Mathf.Abs(WorkFoodCost - globalStats.GetComponent<GlobalStats>().AvrageWorkFoodCost);


        gfSum = GfCostSearchRadius + GfCostAgentSpeed + GfCostSpeedCost + GfCostWorkFoodCost + BaseGfCost;

        GodForceCosts[0] = GfCostSearchRadius;
        GodForceCosts[1] = GfCostAgentSpeed;
        GodForceCosts[2] = GfCostSpeedCost;
        GodForceCosts[3] = GfCostWorkFoodCost;
        GodForceCosts[4] = gfSum;



        if (creatingGodAngel)
        {
            
            Time.timeScale = 0f;


            if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, 15f, LayerMask.GetMask("Ground")))
            {
                v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

                v3 = Camera.main.ScreenToWorldPoint(v3);

                v3.z = 0f;

                newBornAngel.transform.position = v3;  
            }

            if (Input.GetMouseButtonDown(0))
            {
                globalStats.GetComponent<GlobalStats>().GodForce -= gfSum;

                globalStats.GetComponent<GlobalStats>().GodAngelsCreated += 1;
                

                creatingGodAngel = false;
                Time.timeScale = lastTimeScale;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(newBornAngel);
                creatingGodAngel = false;
                Time.timeScale = lastTimeScale;
            }
        }
    }

    public void CreateAngelFunc()
    {
        if (globalStats.GetComponent<GlobalStats>().GodForce > gfSum)
        {
            creatingGodAngel = true;

            lastTimeScale = Time.timeScale;

            creationPoint.Set(20f, 0.4f, 0f);

            newBornAngel = Instantiate(AngelPrefab, creationPoint, rotation);


            newBornAngel.GetComponent<GodAngel>().SearchRadius = SearchRadius;
            newBornAngel.GetComponent<GodAngel>().AgentSpeed = AgentSpeed;
            newBornAngel.GetComponent<GodAngel>().SpeedCost = SpeedCost;
            newBornAngel.GetComponent<GodAngel>().WorkFoodCost = WorkFoodCost;
            newBornAngel.GetComponent<GodAngel>().WorkEnergyCost = WorkEnergyCost;

            newBornAngel.GetComponent<GodAngel>().JustBorn = true;

            newBornAngel.GetComponent<GodAngel>().wantsToMate = false;
            newBornAngel.GetComponent<GodAngel>().foundMate = false;
            newBornAngel.GetComponent<GodAngel>().horney = 1f;
            newBornAngel.GetComponent<GodAngel>().reproductiveUrge = 0.0f;

            newBornAngel.GetComponent<GodAngel>().GodAngelColor = GodAngelColor;
            SpriteRenderer _renderer = newBornAngel.transform.GetChild(2).GetComponent<SpriteRenderer>();
            _renderer.color = GodAngelColor;
        }
    }
}
