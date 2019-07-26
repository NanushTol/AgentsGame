using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using static Constants;

public class SelectObject : MonoBehaviour
{
    
    public GameObject DdAgentType;
    public GameObject agentPropertiesUi;

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
        
        int workLayer = 1 << LayerMask.NameToLayer("Work");
        int agentLayer = 1 << LayerMask.NameToLayer("Agent");
        int foodLayer = 1 << LayerMask.NameToLayer("Food");
        int resourceLayer = 1 << LayerMask.NameToLayer("Resource");

        mask = workLayer | agentLayer | foodLayer | resourceLayer;
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
                    agentPropertiesUi.SetActive(true);
                    agentIsSelected = true;
                    SelectedAgent = SelectedObject;
                    DdAgentType.GetComponent<GnobTypeDropdown>().UpdateType();
                    GetAgentProperties();
                }
                else
                {
                    agentPropertiesUi.SetActive(false);
                    agentIsSelected = false;
                }
            }
            
            // No Hit
            else
            {
                SelectedObject = null;
                PlaceIndicator(new Vector3(0f,-100f,0f));
                agentPropertiesUi.SetActive(false);
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
        Agent agent = SelectedObject.GetComponent<Agent>();
        AgentAge = agent.CurrentAge;
        AgentType = agent.AgentType;
        AgentMostUrgentNeed = agent.MostUrgentNeed;
        AgentInBuilding = agent.InBuilding;

        AgentProperties[0] = agent.NeedsManager.NeedsValues[HUNGRY];
        AgentProperties[1] = agent.NeedsManager.NeedsValues[HORNY];
        AgentProperties[2] = agent.Energy / 100f;

    }
}

