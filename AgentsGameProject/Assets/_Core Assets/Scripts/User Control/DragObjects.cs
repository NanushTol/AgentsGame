using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DragObjects : MonoBehaviour
{

    Transform transformToDrag;
    float distance;
    Vector3 offset;
    bool dragging;
    Vector3 v3;
    int mask;
    Tilemap tileMap;
    int mapWidth;
    int mapHeight;

    MapCreator mapCreator;

    // Start is called before the first frame update
    void Awake()
    {
        mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();

        tileMap = GameObject.Find("Tilemap_BaseWater").GetComponent<Tilemap>();
        int work = 1 << LayerMask.NameToLayer("Work");
        int agent = 1 << LayerMask.NameToLayer("Agent");
        int godAngel = 1 << LayerMask.NameToLayer("GodAngel");
        //int food = 1 << LayerMask.NameToLayer("Food");
        mask = agent | godAngel;
        mapWidth = tileMap.size.x;
        mapHeight = tileMap.size.y;
        //Debug.Log("map width " + mapWidth);
        //Debug.Log("map height " + mapHeight);

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
                //Debug.Log("hit " + hit.collider.gameObject);
                transformToDrag = hit.transform;
                distance = hit.transform.position.z - Camera.main.transform.position.z;
                v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
                v3 = Camera.main.ScreenToWorldPoint(v3);

                offset = transformToDrag.position - v3;

                dragging = true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (dragging)
            {
                if(transformToDrag != null)
                {
                    if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, 15f, LayerMask.GetMask("Ground")))
                    {
                        v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);

                        v3 = Camera.main.ScreenToWorldPoint(v3);

                        if (transformToDrag.gameObject.tag != "Work")
                        {
                            transformToDrag.position = v3 + offset;
                        }
                        if (transformToDrag.gameObject.tag == "Work")
                        {
                            v3 = transformToDrag.GetComponent<GenericBuilding>().GridRef.WorldToCell(v3);

                            v3.x += 0.5f;
                            v3.y += 0.5f;

                            transformToDrag.position = v3;

                            Vector3Int _lastPosition = transformToDrag.GetComponent<GenericBuilding>().LastPosition; //get last node position
                            transformToDrag.GetComponent<GenericBuilding>().UpdateNode(_lastPosition, true); // set last position node to walkable
                        }
                    }
                }
                if(transformToDrag == null)
                {
                    dragging = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (dragging)
            {
                dragging = false;
                if (transformToDrag.gameObject.tag == "Work")
                {
                    Vector3Int position = transformToDrag.GetComponent<GenericBuilding>().GridRef.WorldToCell(transformToDrag.position);

                    position.x += mapCreator.MapWidth / 2;
                    position.y += (mapCreator.MapHeight / 2) - 1;

                    transformToDrag.GetComponent<GenericBuilding>().LastPosition = position;

                    transformToDrag.GetComponent<GenericBuilding>().UpdateNode(position, false);
                }

                transformToDrag = null;
            }
        }
    }
}
