using UnityEngine;

[CreateAssetMenu]
public class AgentsSharedParameters : ScriptableObject
{
    public GameObject AgentPrefab;

    public float ConsumptionScale = 1f;
    public float MaxEnergy = 100;
    public float MaxFood = 100;
    public float ReproductiveRecoveryTime = 15f;
    public float AgentSpeed = 10f;
    public float SearchRadius = 20f;
    public float BiteSize = 5f;
    public float SleepEfficiency = 5f;
    public float FoodFullThreshold = 80f;
    public float AwakeThreshold = 90f;
    public float MutaionPercentage = 0.15f;
    public float GfPerSecond = 0.05f;
}
