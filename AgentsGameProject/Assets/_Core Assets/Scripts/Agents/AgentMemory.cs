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

    public Dictionary<Agent, float> PotentialMates = new Dictionary<Agent, float>();

    public Dictionary<Food, float> FoodPlaces = new Dictionary<Food, float>();

    public List<SleepPlace> SleepingPlaces = new List<SleepPlace>();

    public List<GameObject> WorkplacesBeliefs = new List<GameObject>();



    public void AddItemToDictionary(GameObject item, Dictionary<GameObject, float> dic)
    {
        dic.Add(item, 0f);
    }

    public void AddItemToDictionary(GenericBuilding item, Dictionary<GenericBuilding, float> dic)
    {
        dic.Add(item, 0f);
    }

    public void AddItemToDictionary(Agent item, Dictionary<Agent, float> dic)
    {
        dic.Add(item, 0f);
    }

    public void AddItemToDictionary(Food item, Dictionary<Food, float> dic)
    {
        dic.Add(item, 0f);
    }

    public void AddItemToList(SleepPlace item, List<SleepPlace> list)
    {
        list.Add(item);
    }
}
