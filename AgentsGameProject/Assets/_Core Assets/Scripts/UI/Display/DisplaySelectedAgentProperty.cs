﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplaySelectedAgentProperty : MonoBehaviour
{
    public enum PropertyToDisplay {Age, Type, Hunger, HungerBar, Energy, EnergBar, Horny, HornyBar, AgentType, Doing }
    public PropertyToDisplay propertyToDisplay;

    SelectObject selectObjectScript;

    [HideInInspector]
    public float AgentHunger;
    [HideInInspector]
    public float AgentTired;
    [HideInInspector]
    public float AgentHorny;
    [HideInInspector]
    public float AgentReadyForWork;
    [HideInInspector]
    public float AgentEnergy;

    int index;

    void Awake()
    {
        selectObjectScript = GameObject.Find("GameManager").GetComponent<SelectObject>();
    }

    void OnEnable()
    {
        switch (propertyToDisplay)
        {
            case PropertyToDisplay.Hunger:
                index = 0;
                break;

            case PropertyToDisplay.Horny:
                index = 1;
                break;

            case PropertyToDisplay.Energy:
                index = 2;
                break;

            case PropertyToDisplay.HungerBar:
                index = 0;
                break;

            case PropertyToDisplay.HornyBar:
                index = 1;
                break;

            case PropertyToDisplay.EnergBar:
                index = 2;
                break;
        }

    }
    void Update()
    {
        if(propertyToDisplay == PropertyToDisplay.Age)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(selectObjectScript.AgentAge).ToString("0");
        }
        else if (propertyToDisplay == PropertyToDisplay.Doing)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = selectObjectScript.AgentMostUrgentNeed;
        }
        else if(propertyToDisplay == PropertyToDisplay.HungerBar || propertyToDisplay == PropertyToDisplay.HornyBar || propertyToDisplay == PropertyToDisplay.EnergBar)
        {
            float mapedBar = Remap(selectObjectScript.AgentProperties[index], 0f, 1f, 0f, 120f);

            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapedBar);
            
        }
        else if (propertyToDisplay == PropertyToDisplay.Hunger || propertyToDisplay == PropertyToDisplay.Horny || propertyToDisplay == PropertyToDisplay.Energy)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(selectObjectScript.AgentProperties[index]).ToString("0" + "%");
        }
        
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        float _remapedValue = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        return _remapedValue;
    }
}