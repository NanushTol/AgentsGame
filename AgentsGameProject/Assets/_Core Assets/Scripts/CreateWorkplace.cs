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

            mousePos = Input.mousePosition;
            newWorkplace.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
            Vector3 positionOffset = new Vector3 (newWorkplace.transform.position.x, newWorkplace.transform.position.y,0f);

            newWorkplace.transform.position = positionOffset;

            if (Input.GetMouseButtonDown(0))
            {
                //CreationPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zOffset));

                globalStats.GetComponent<GlobalStats>().GodForce -= BaseGfCost;

                creatingWorkplace = false;
                Time.timeScale = 1f;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
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
