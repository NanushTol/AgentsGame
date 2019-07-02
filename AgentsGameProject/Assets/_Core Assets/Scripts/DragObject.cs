using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class DragObject : MonoBehaviour

{
    GameObject globalStats;
    private Vector3 mOffset;

    Vector3 lastPosition;

    float GfCost = 0.07f;

    private float mZCoord;

    Vector3 worldPosition;

    private void Awake()
    {
        //globalStats = GameObject.Find("GlobalStats");
        //Physics.queriesHitTriggers = false;
    }

    /*
    void Update()
    {
        //Physics.queriesHitTriggers = false;

        if (Input.GetMouseButtonDown(0))
        {
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            // Store offset = gameobject world pos - mouse world pos
            mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

            lastPosition = gameObject.transform.position;


            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f, gameObject.layer, QueryTriggerInteraction.Ignore) && Input.GetMouseButton(0))
            {
                //if (globalStats.GetComponent<GlobalStats>().moving)
                //{
                //if (globalStats.GetComponent<GlobalStats>().GodForce >= GfCost)
                //{

                transform.position = GetMouseAsWorldPoint() + mOffset;

                //globalStats.GetComponent<GlobalStats>().GodForce -= Mathf.Abs((transform.position - lastPosition).magnitude) * GfCost;

                lastPosition = transform.position;
                //}
                //}
            }
        }
    }
    */

    private void OnMouseOver()
    {
        //Debug.Log("mouse Over " + gameObject.name);
    }



    void OnMouseDown()

    {
        //if (globalStats.GetComponent<GlobalStats>().moving)
        //{
        //}
        //Physics.queriesHitTriggers = false;

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z; ;
            

        // Store offset = gameobject world pos - mouse world pos

        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();


        lastPosition = gameObject.transform.position;

    }
    


    private Vector3 GetMouseAsWorldPoint()

    {

        // Pixel coordinates of mouse (x,y)

        Vector3 mousePoint = Input.mousePosition;



        // z coordinate of game object on screen

        mousePoint.z = mZCoord;



        // Convert it to world points

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }


    
    void OnMouseDrag()
    {
        //Physics.queriesHitTriggers = false;
        //if (globalStats.GetComponent<GlobalStats>().moving)
        //{
        //if (globalStats.GetComponent<GlobalStats>().GodForce >= GfCost)
        //{
        //globalStats.GetComponent<GlobalStats>().GodForce -= Mathf.Abs((transform.position - lastPosition).magnitude) * GfCost;
        //}
        //}
        if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground")))
        {
            if (gameObject.tag != "Work")
            {
                transform.position = GetMouseAsWorldPoint() + mOffset;
            }

            if (gameObject.tag == "Work")
            {
                worldPosition = GetMouseAsWorldPoint() + mOffset;
                Vector3 position = GetComponent<WorkPlace>().grid.WorldToCell(worldPosition);

                position.x += 0.5f;
                position.y += 0.5f;

                transform.position = position;
            }

            lastPosition = transform.position;

            if (gameObject.tag == "Work")
            {
                Vector3Int _lastPosition = GetComponent<WorkPlace>().LastPosition; //get last node position
                GetComponent<WorkPlace>().UpdateNode(_lastPosition, true); // set last position node to walkable
            }
        }
    }


    private void OnMouseUp()
    {
        if (gameObject.tag == "Work")
        {
            Vector3Int position = GetComponent<WorkPlace>().grid.WorldToCell(transform.position);

            position.x += 18;
            position.y += 17;

            GetComponent<WorkPlace>().LastPosition = position;

            GetComponent<WorkPlace>().UpdateNode(position, false);
        }
    }
}