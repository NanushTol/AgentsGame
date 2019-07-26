using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentDecisionMaker : MonoBehaviour
{
    AgentMemory agentMemory;

    private void Awake()
    {
        agentMemory = GetComponent<AgentMemory>();
    }


    public void ScoreItemsInDictionary(Dictionary<GenericBuilding, float> dictionary)
    {
        foreach (KeyValuePair<GenericBuilding, float> dicEntry in dictionary.ToList())
        {
            dictionary[dicEntry.Key] = TempBeliefDistance(dicEntry.Key, this.gameObject);
        }
    }
    public void ScoreItemsInDictionary(Dictionary<Agent, float> dictionary)
    {
        foreach (KeyValuePair<Agent, float> dicEntry in dictionary.ToList())
        {
            dictionary[dicEntry.Key] = TempBeliefDistance(dicEntry.Key, this.gameObject);
        }
    }
    public void ScoreItemsInDictionary(Dictionary<Food, float> dictionary)
    {
        foreach (KeyValuePair<Food, float> dicEntry in dictionary.ToList())
        {
            dictionary[dicEntry.Key] = TempBeliefDistance(dicEntry.Key, this.gameObject);
        }
    }



    public Dictionary<GenericBuilding, float> SortDictionaryByValues(Dictionary<GenericBuilding, float> dictionary)
    {
        var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;
        
        return sortedDict.ToDictionary(x => x.Key, x => x.Value);
    }
    public Dictionary<Agent, float> SortDictionaryByValues(Dictionary<Agent, float> dictionary)
    {
        var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;

        return sortedDict.ToDictionary(x => x.Key, x => x.Value);
    }
    public Dictionary<Food, float> SortDictionaryByValues(Dictionary<Food, float> dictionary)
    {
        var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;

        return sortedDict.ToDictionary(x => x.Key, x => x.Value);
    }



    float TempBeliefDistance(GenericBuilding workplace, GameObject source)
    {
        float score = 0;

        score = Mathf.Abs(Vector3.Distance(source.transform.position, workplace.transform.position));

        return score;
    }
    float TempBeliefDistance(Agent agent, GameObject source)
    {
        float score = 0;

        score = Mathf.Abs(Vector3.Distance(source.transform.position, agent.transform.position));

        return score;
    }
    float TempBeliefDistance(Food food, GameObject source)
    {
        float score = 0;

        score = Mathf.Abs(Vector3.Distance(source.transform.position, food.transform.position));

        return score;
    }
}
