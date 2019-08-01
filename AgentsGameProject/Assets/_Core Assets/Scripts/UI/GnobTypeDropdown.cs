using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GnobTypeDropdown : MonoBehaviour
{
    SelectObject selectObjectScript;
    string AgentType;
    int DdIndex;

    void Awake()
    {
        selectObjectScript = GameObject.Find("UserControls").GetComponent<SelectObject>();
    }

    public void UpdateType()
    {
        AgentType = selectObjectScript.Agent.AgentType;

        switch (AgentType)
        {
            case "Worker":
                DdIndex = 0;
                break;

            case "Farmer":
                DdIndex = 1;
                break;

            case "Builder":
                DdIndex = 2;
                break;

            case "Prist":
                DdIndex = 3;
                break;

            case "Transporter":
                DdIndex = 4;
                break;

            case "Scientist":
                DdIndex = 5;
                break;
        }

        this.GetComponent<TMP_Dropdown>().value = DdIndex;

    }

    public void ChangeGnobType()
    {
        DdIndex = this.GetComponent<TMP_Dropdown>().value;

        switch (DdIndex)
        {
            case 0:
                AgentType = "Worker";
                break;

            case 1:
                AgentType = "Farmer";
                break;

            case 2:
                AgentType = "Builder";
                break;

            case 3:
                AgentType = "Prist";
                break;

            case 4:
                AgentType = "Transporter";
                break;

            case 5:
                AgentType = "Scientist";
                break;
        }

        selectObjectScript.Agent.AgentType = AgentType;
    }
}
