using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateWorkplace : MonoBehaviour
{
    public GameObject WorkplacePrefab;
    public GameObject globalStats;
    public float BaseGfCost;
    public Vector2 mousePos;

    #region //local variables
    float gfSum;

    GameObject newWorkplace;
    //public Vector3 CreationPosition;
    Quaternion rotation = new Quaternion(0, 0, 0, 0);
    RaycastHit hit;
    float yOffset = 5f;
    bool creatingWorkplace;

    Vector3 v3;
    float distance;
    Vector3 offset;

    #endregion
    private void Awake()
    {

    }
    void Update()
    {
        if (creatingWorkplace)
        {
            
            Time.timeScale = 0f;
            float zOffset = Camera.main.transform.position.z;
                //Camera.main.nearClipPlane + Camera.main.transform.position.z;
            //Camera.main.transform.position.z * (-1);
                //

            //mousePos = Input.mousePosition;
            //newWorkplace.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
            //Vector3 positionOffset = new Vector3 (newWorkplace.transform.position.x, newWorkplace.transform.position.y,0f);
            //newWorkplace.transform.position = positionOffset;

            if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, 15f, LayerMask.GetMask("Ground")))
            {
                v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);

                v3 = Camera.main.ScreenToWorldPoint(v3);

                v3 = newWorkplace.GetComponent<WorkPlace>().grid.WorldToCell(v3);

                v3.x += 0.5f;
                v3.y += 0.5f;

                newWorkplace.transform.position = v3;

                //Vector3Int _lastPosition = newWorkplace.GetComponent<WorkPlace>().LastPosition; //get last node position
                //newWorkplace.GetComponent<WorkPlace>().UpdateNode(_lastPosition, true); // set last position node to walkable    

            }

            if (Input.GetMouseButtonDown(0))
            {
                //CreationPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zOffset));

                globalStats.GetComponent<GlobalStats>().GodForce -= BaseGfCost;

                Vector3Int position = newWorkplace.GetComponent<WorkPlace>().grid.WorldToCell(newWorkplace.transform.position);

                position.x += 18;
                position.y += 17;

                newWorkplace.GetComponent<WorkPlace>().LastPosition = position;

                newWorkplace.GetComponent<WorkPlace>().UpdateNode(position, false);
                creatingWorkplace = false;
                Time.timeScale = 1f;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(newWorkplace);
                creatingWorkplace = false;
                Time.timeScale = 1f;
            }
        }
    }
    public void CreateWorkplaceFunc()
    {
        if(globalStats.GetComponent<GlobalStats>().GodForce > BaseGfCost)
        {
            newWorkplace = Instantiate(WorkplacePrefab, new Vector3(0f, 0f, -5f), rotation);
            creatingWorkplace = true;
        }
    }
}
