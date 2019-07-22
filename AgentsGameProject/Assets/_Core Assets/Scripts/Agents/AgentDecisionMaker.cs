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

    public Dictionary<GameObject, float> SortDictionaryByValues(Dictionary<GameObject, float> dictionary)
    {
        foreach (KeyValuePair<GameObject, float> dicEntry in dictionary)
        {
            dictionary[dicEntry.Key] = TempBeliefDistance(dicEntry.Key, this.gameObject);
        }

        // Sort WorkPlaces By Scores
        var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;

        return (Dictionary<GameObject, float>)sortedDict;
    }
    public Dictionary<GenericBuilding, float> SortDictionaryByValues(Dictionary<GenericBuilding, float> dictionary)
    {
        foreach (KeyValuePair<GenericBuilding, float> dicEntry in dictionary)
        {
            dictionary[dicEntry.Key] = TempBeliefDistance(dicEntry.Key, this.gameObject);
        }

        // Sort WorkPlaces By Scores
        var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;

        return (Dictionary<GenericBuilding, float>)sortedDict;
    }


    float TempBeliefDistance(GameObject workplace, GameObject source)
    {
        float score = 0;

        score = Mathf.Abs(Vector3.Distance(source.transform.position, workplace.transform.position));

        return score;
    }
    float TempBeliefDistance(GenericBuilding workplace, GameObject source)
    {
        float score = 0;

        score = Mathf.Abs(Vector3.Distance(source.transform.position, workplace.transform.position));

        return score;
    }
}
