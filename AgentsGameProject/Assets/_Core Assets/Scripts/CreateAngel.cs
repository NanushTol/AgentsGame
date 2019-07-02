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
    public GameObject globalStats;
    public float BaseGfCost;

    public float GfCostSearchRadius;
    public float GfCostAgentSpeed;
    public float GfCostSpeedCost;
    public float GfCostWorkFoodCost;
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
    #endregion

    #region //local variables
    float gfSum;

    GameObject newBornAngel;
    Vector3 birthPosition;
    Quaternion rotation = new Quaternion(0, 0, 0, 0);
    RaycastHit hit;
    float yOffset = 5f;
    Vector3 creationPoint;
    #endregion
    private void Awake()
    {
        GodForceCosts = new float[5];
    }
    void Update()
    {
        //Get Input Traits
        SearchRadius = float.Parse(SearchRadiusField.GetComponent<TMP_InputField>().text);
        AgentSpeed = float.Parse(SpeedField.GetComponent<TMP_InputField>().text);
        SpeedCost = float.Parse(SpeedCostField.GetComponent<TMP_InputField>().text);
        WorkFoodCost = float.Parse(WorkFoodCostField.GetComponent<TMP_InputField>().text);

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
    }
    public void CreateAngelFunc()
    {
        if (globalStats.GetComponent<GlobalStats>().GodForce > 0)
        {
            creationPoint.Set(20f, 0.4f, 0f);

            newBornAngel = Instantiate(AngelPrefab, creationPoint, rotation);

            globalStats.GetComponent<GlobalStats>().GodAngelsCreated += 1;
            globalStats.GetComponent<GlobalStats>().GodForce -= gfSum;


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
        }
        #region // create on mouse position
        //Time.timeScale = 0;
        /*
        if (Input.GetMouseButtonDown(0)) // Create a GodAngel on mouse click position
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    creationPoint.Set(hit.point.x, yOffset, hit.point.z);
                    Time.timeScale = 1;
                    newBornAngel = Instantiate(AngelPrefab, creationPoint, rotation);
                }
            }
        }
        */
        #endregion 
    }
}
