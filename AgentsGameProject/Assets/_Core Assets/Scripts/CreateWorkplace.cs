using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateWorkplace : MonoBehaviour
{
    public GameObject WorkplacePrefab;
    public GameObject gameManager;
    public float BaseGfCost;
    public Vector2 mousePos;

    #region //local variables
    float gfSum;

    GameObject newWorkplace;
    //public Vector3 CreationPosition;
    Quaternion rotation = new Quaternion(0, 0, 0, 0);
    RaycastHit hit;
    float yOffset = 5f;
    public bool creatingWorkplace = false;

    float lastTimeScale;
    Vector3 v3;
    float distance;
    Vector3 offset;
    #endregion

    void Update()
    {
        if (creatingWorkplace)
        {
            Time.timeScale = 0f;

            float zOffset = Camera.main.transform.position.z;

            // Ground Check
            if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, 15f, LayerMask.GetMask("Ground")))
            {
                v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);

                v3 = Camera.main.ScreenToWorldPoint(v3);

                v3 = newWorkplace.GetComponent<GenericBuilding>().grid.WorldToCell(v3);

                v3.x += 0.5f;
                v3.y += 0.5f;

                newWorkplace.transform.position = v3;
            }

            // left mouse Click
            if (Input.GetMouseButtonDown(0))
            {
                //CreationPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zOffset));

                gameManager.GetComponent<ResourcesData>().GodForceAmount -= BaseGfCost;

                Vector3Int position = newWorkplace.GetComponent<GenericBuilding>().grid.WorldToCell(newWorkplace.transform.position);

                position.x += 18;
                position.y += 17;

                newWorkplace.GetComponent<GenericBuilding>().LastPosition = position;

                newWorkplace.GetComponent<GenericBuilding>().UpdateNode(position, false);
                creatingWorkplace = false;
                Time.timeScale = lastTimeScale;
            }

            // Escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(newWorkplace);
                creatingWorkplace = false;
                Time.timeScale = lastTimeScale;
            }
        }
    }



    public void CreateWorkplaceFunc()
    {
        if(gameManager.GetComponent<ResourcesData>().GodForceAmount > BaseGfCost)
        {
            newWorkplace = Instantiate(WorkplacePrefab, new Vector3(0f, 0f, -5f), rotation);
            creatingWorkplace = true;
            lastTimeScale = Time.timeScale;
        }
    }
}
