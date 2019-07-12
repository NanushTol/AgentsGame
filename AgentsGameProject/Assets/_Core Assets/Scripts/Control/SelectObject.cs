using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectObject : MonoBehaviour
{
    [HideInInspector]
    public GameObject SelectedObject;

    [HideInInspector]
    GameObject selectionIndicator;

    int mask;
  

    // Start is called before the first frame update
    void Awake()
    {
        selectionIndicator = GameObject.Find("SelectionIndicator");
        
        int work = 1 << LayerMask.NameToLayer("Work");
        int agent = 1 << LayerMask.NameToLayer("Agent");
        int godAngel = 1 << LayerMask.NameToLayer("GodAngel");
        int food = 1 << LayerMask.NameToLayer("Food");
        int resource = 1 << LayerMask.NameToLayer("Resource");

        mask = work | agent | godAngel | food | resource;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Camera.main.transform.forward, 15f, mask);
            if (hit)
            {
                SelectedObject = hit.transform.gameObject;
            }
            else
            {
                SelectedObject = null;
                PlaceIndicator(new Vector3(0f,-100f,0f));
            }
        }

        if (SelectedObject)
        {
            PlaceIndicator(SelectedObject.transform.position);
        }
    }

    void PlaceIndicator(Vector3 _position)
    {
        selectionIndicator.transform.position = _position;
    }
}

