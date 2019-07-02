using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateWall : MonoBehaviour
{
    public GameObject WallPrefab;
    public GameObject globalStats;
    public float BaseGfCost;
    
    #region //local variables
    float gfSum;

    GameObject newWall;
    Quaternion rotation = new Quaternion(0, 0, 0, 0);
    bool creatingWall;
    float lastTimeScale;
    #endregion
    private void Awake()
    {
    }
    void Update()
    {
        if (creatingWall)
        {
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            float zOffset = Camera.main.nearClipPlane + Camera.main.transform.position.y;

            Vector2 mousePos = Input.mousePosition;
            newWall.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zOffset));

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 CreationPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zOffset));

                globalStats.GetComponent<GlobalStats>().GodForce -= BaseGfCost;

                creatingWall = false;
                Time.timeScale = lastTimeScale;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                creatingWall = false;
                Time.timeScale = lastTimeScale;
            }
        }
    }
    public void CreateWallFunc()
    {
        if(globalStats.GetComponent<GlobalStats>().GodForce > BaseGfCost)
        {
            newWall = Instantiate(WallPrefab, new Vector3(0f, -5f, 0f), rotation);
            creatingWall = true;
        }
    }
}
