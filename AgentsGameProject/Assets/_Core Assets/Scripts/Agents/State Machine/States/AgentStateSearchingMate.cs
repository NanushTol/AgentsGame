using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateSearchingMate : IAgentState
{
    public Agent Owner;

    public AgentStateSearchingMate(Agent agent) { this.Owner = agent; }

    public String StateName
    {
        get { return "Searching Mate"; }

    }
    public Color StateColor
    {
        get { return Color.magenta; }
    }

    public string LayerMaskString = "Agent";
    public string Tag = "Agent";

    private float _elapsedTimeFind = 1.5f;
    private float _timerFind = 1f;

    bool _sumTimer = false;
    private float _elapsedTimeSum = 0;
    private float _timerSum = 0.03f;

    bool _checkMatches = false;

    #region // State Functions

    public void Enter()
    {
        Owner.State = StateName;
        Owner.ActiveState = Agent.StatesEnum.SearchingMate;
        _elapsedTimeFind = 1.5f;
        _elapsedTimeSum = 0;
        _sumTimer = false;
        _checkMatches = false;
    }

    public void ExecuteState()
    {
        _elapsedTimeFind += Time.deltaTime;

        if (_elapsedTimeFind >= _timerFind)
        {
            FindPotentialMates();

            // Score Mates
            Owner.DecisionMaker.ScoreItemsInDictionary(Owner.AgentMemory.PotentialMates);

            if (Owner.AgentMemory.PotentialMates.Count >= 1)
            {
                _elapsedTimeFind = 0f;
                _sumTimer = true;
            }
        }

        if (_sumTimer) _elapsedTimeSum += Time.deltaTime;

        if (_elapsedTimeSum >= _timerSum)
        {
            // Sum Potential Mates Scores of Owner With Owner Scores
            SumMatesScores();

            // Sort PotentialMates
            Owner.AgentMemory.PotentialMates = Owner.DecisionMaker.SortDictionaryByValues(Owner.AgentMemory.PotentialMates);

            _elapsedTimeSum = 0f;
            _sumTimer = false;
            _checkMatches = true;
        }
            

        if (_checkMatches && Owner.AgentMemory.PotentialMates.Count >= 1)
        {
            // Check If there is a match
            GameObject chosenMate = CheckMatch();
            if (chosenMate != null)
            {
                Owner.ChosenMate = chosenMate;
                // Switch State to Move To Mate
                Owner.StateMachine.ChangeState(Owner.States[Agent.StatesEnum.MovingToMate]);
                _checkMatches = false;
            }
        }
    }

    public void Exit()
    {
        
    }

    public void OnTriggerStay(Collider2D collider)
    {

    }

    #endregion

    #region // Extention Functions

    void FindPotentialMates()
    {
        Owner.AgentMemory.PotentialMates.Clear();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(Owner.transform.position, Owner.AgentsSharedParameters.SearchRadius, LayerMask.GetMask(LayerMaskString));

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != Owner.gameObject)
            {
                // if Other Agent is also searching for mate
                if (collider.GetComponent<Agent>().ActiveState == Agent.StatesEnum.SearchingMate)
                {
                    Owner.AgentMemory.AddItemToDictionary(collider.GetComponent<Agent>(), Owner.AgentMemory.PotentialMates);
                }
            }
            
        }
    }

    public void SumMatesScores()
    {
        foreach (KeyValuePair<Agent, float> otherAgent in Owner.AgentMemory.PotentialMates.ToList())
        {
            float sum;

            if (otherAgent.Key)
            {
                if(otherAgent.Key.AgentMemory.PotentialMates.ContainsKey(Owner))
                {
                    sum = Owner.AgentMemory.PotentialMates[otherAgent.Key] + otherAgent.Key.AgentMemory.PotentialMates[Owner];

                    Owner.AgentMemory.PotentialMates[otherAgent.Key] = sum; 
                }
            }
            else Owner.AgentMemory.PotentialMates[otherAgent.Key] = 0f;
        }
    }

    public GameObject CheckMatch()
    {
        Agent firstInOwnerList = Owner.AgentMemory.PotentialMates.ElementAt(0).Key;

        if (firstInOwnerList.AgentMemory.PotentialMates.Count >= 1)
        {
            Agent firstInMateList = firstInOwnerList.AgentMemory.PotentialMates.ElementAt(0).Key;

            if (firstInMateList == Owner)
                return firstInOwnerList.gameObject;
            else
                return null;
        }
        else
            return null;
    }

    #endregion
}
