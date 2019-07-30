using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

public class DisplaySelectedWorkProperty : MonoBehaviour
{
    public enum PropertyToDisplay {BuildingActive, ResourceLeft, ResourceLeftBar, AgentWorking, AgentsWorkingBar, UpKeep, Production, BuildingType, ResourceType}
    public PropertyToDisplay propertyToDisplay;

    SelectObject selectObjectScript;

    [HideInInspector]
    public Color ResourceColor;

    int index;

    void Awake()
    {
        selectObjectScript = GameObject.Find("GameManager").GetComponent<SelectObject>();
    }

    void OnEnable()
    {
        switch (propertyToDisplay)
        {
            case PropertyToDisplay.BuildingActive:
                index = (int)PropertyToDisplay.BuildingActive;
                break;

            case PropertyToDisplay.ResourceLeft:
                index = (int)PropertyToDisplay.ResourceLeft;
                break;

            case PropertyToDisplay.ResourceLeftBar:
                index = (int)PropertyToDisplay.ResourceLeftBar;
                break;

            case PropertyToDisplay.AgentWorking:
                index = (int)PropertyToDisplay.AgentWorking;
                break;

            case PropertyToDisplay.AgentsWorkingBar:
                index = (int)PropertyToDisplay.AgentsWorkingBar;
                break;

            case PropertyToDisplay.UpKeep:
                index = (int)PropertyToDisplay.UpKeep;
                break;

            case PropertyToDisplay.Production:
                index = (int)PropertyToDisplay.Production;
                break;

            case PropertyToDisplay.BuildingType:
                index = (int)PropertyToDisplay.BuildingType;
                break;

            case PropertyToDisplay.ResourceType:
                index = (int)PropertyToDisplay.ResourceType;
                ResourceColor = selectObjectScript.ResourceColor.ColorValue;
                break;
        }

    }
    void Update()
    {
        if(propertyToDisplay == PropertyToDisplay.ResourceLeft || propertyToDisplay == PropertyToDisplay.AgentWorking)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(selectObjectScript.BuildingProperties[index]).ToString("0");
        }

        else if (propertyToDisplay == PropertyToDisplay.BuildingType)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = selectObjectScript.BuildingType.Value;
        }

        else if (propertyToDisplay == PropertyToDisplay.ResourceType)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = selectObjectScript.ResourceType + ":";
        }

        else if(propertyToDisplay == PropertyToDisplay.AgentsWorkingBar)
        {
            float mapedBar = Remap(selectObjectScript.BuildingProperties[index], 0f, selectObjectScript.BuildingBarMaxValue[index], 0f, 120f);

            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapedBar);
            
        }

        else if (propertyToDisplay == PropertyToDisplay.ResourceLeftBar)
        {
            float mapedBar = Remap(selectObjectScript.BuildingProperties[index], 0f, selectObjectScript.BuildingBarMaxValue[index], 0f, 120f);

            this.GetComponent<Image>().color = ResourceColor;
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapedBar);
        }

        else if (propertyToDisplay == PropertyToDisplay.UpKeep || propertyToDisplay == PropertyToDisplay.Production)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(selectObjectScript.BuildingProperties[index]).ToString("0.00");
        }

        else if(propertyToDisplay == PropertyToDisplay.BuildingActive)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = selectObjectScript.BuildingActive.ToString();
        }
        
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        float _remapedValue = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        return _remapedValue;
    }
}
