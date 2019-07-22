using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Memory of the agent
/// It is a component that sits on the agent's GameObject
/// </summary>
public class AgentMemory : MonoBehaviour
{
    public Dictionary<GenericBuilding , float> Workplaces = new Dictionary<GenericBuilding, float>();

    public Dictionary<GameObject, float> PotentialMate = new Dictionary<GameObject, float>();

    public Dictionary<GameObject, float> EatingPlaces = new Dictionary<GameObject, float>();

    public List<GameObject> SleepingPlaces = new List<GameObject>();



    public List<GameObject> WorkplacesBeliefs = new List<GameObject>();



    public void AddItemToDictionary(GameObject item, Dictionary<GameObject, float> dic)
    {
        dic.Add(item, 0f);
    }

    public void AddItemToDictionary(GenericBuilding item, Dictionary<GenericBuilding, float> dic)
    {
        dic.Add(item, 0f);
    }
}
