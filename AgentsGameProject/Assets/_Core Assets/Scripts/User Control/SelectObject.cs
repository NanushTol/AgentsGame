using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class SelectObject : MonoBehaviour
{
    
    public GameObject DdAgentType;
    public GameObject gnobProperties;

    [HideInInspector]
    GameObject selectionIndicator;
    [HideInInspector]
    public GameObject SelectedObject;

    int mask;

    #region // Agents Properties
    [HideInInspector]
    public float AgentDoing;
    [HideInInspector]
    public string AgentType;
    [HideInInspector]
    public string AgentMostUrgentNeed;
    [HideInInspector]
    public float AgentAge;
    [HideInInspector]
    public float[] AgentProperties = new float[11];
    [HideInInspector]
    public GameObject SelectedAgent;
    [HideInInspector]
    public bool AgentInBuilding;
    


    #endregion

    bool agentIsSelected = false;

    // Start is called before the first frame update
    void Awake()
    {
        selectionIndicator = GameObject.Find("SelectionIndicator");
        
        //gnobProperties = GameObject.Find("GnobProperties");


        int work = 1 << LayerMask.NameToLayer("Work");
        int agent = 1 << LayerMask.NameToLayer("Agent");
        int godAngel = 1 << LayerMask.NameToLayer("GodAngel");
        int food = 1 << LayerMask.NameToLayer("Food");
        int resource = 1 << LayerMask.NameToLayer("Resource");
        int ui = 1 << LayerMask.NameToLayer("UI");

        mask = work | agent | godAngel | food | resource;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUi())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Camera.main.transform.forward, 15f, mask);
            // if Raycast hit somthing
            if (hit)
            {
                SelectedObject = hit.transform.gameObject;

                if (hit.transform.gameObject.CompareTag("Agent"))
                {
                    gnobProperties.SetActive(true);
                    agentIsSelected = true;
                    SelectedAgent = SelectedObject;
                    DdAgentType.GetComponent<GnobTypeDropdown>().UpdateType();
                    GetAgentProperties();
                }
                else
                {
                    gnobProperties.SetActive(false);
                    agentIsSelected = false;
                }
            }
            
            // No Hit
            else
            {
                SelectedObject = null;
                PlaceIndicator(new Vector3(0f,-100f,0f));
                gnobProperties.SetActive(false);
                agentIsSelected = false;
            }
        }

        if (SelectedObject)
        {
            
            if (agentIsSelected)
            {
                GetAgentProperties();
            }

            if (!AgentInBuilding)
            {
                PlaceIndicator(SelectedObject.transform.position);
            }
            else if (AgentInBuilding)
            {
                PlaceIndicator(new Vector3(0f, -100f, 0f));
            }
           
        }
    }

    void PlaceIndicator(Vector3 _position)
    {
        selectionIndicator.transform.position = _position;
    }

    private bool IsMouseOverUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void GetAgentProperties()
    {
        AgentAge = SelectedObject.GetComponent<Agent>().currentAge;
        AgentType = SelectedObject.GetComponent<Agent>().AgentType;
        AgentMostUrgentNeed = SelectedObject.GetComponent<Agent>().mostUrgentNeed;
        AgentInBuilding = SelectedObject.GetComponent<Agent>().InBuilding;

        AgentProperties[0] = SelectedObject.GetComponent<Agent>().hungry;
        AgentProperties[1] = SelectedObject.GetComponent<Agent>().horney;
        AgentProperties[2] = SelectedObject.GetComponent<Agent>().energy / 100f;

    }
}

