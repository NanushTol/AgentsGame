using UnityEngine;

[CreateAssetMenu]
public class AgentsSharedParameters : ScriptableObject
{
    public GameObject AgentPrefab;

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
    public float MutaionPercentage = 0.15f;
    public float GfPerBirth;
    public float GeaneAvrage;
}
