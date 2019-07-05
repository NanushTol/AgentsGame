using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using Pathfinding;

public class CreateLand : MonoBehaviour
{
    public float LandCost = 3f;
    Grid grid;
    public Tilemap LandTileMap;
    public Tilemap WaterTileMap;
    public TileBase LandTile;
    public TileBase WaterTile;
    public GameObject note;

    GameObject hoverTile;
    Vector3 hoverTileOffset = new Vector3(0.5f, 0.5f, 0f);
    GlobalStats globalStats;



    [HideInInspector]
    public bool creatingLand = false;

    void Awake()
    {
        hoverTile = GameObject.Find("HoverTile");
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        globalStats = GameObject.Find("GlobalStats").GetComponent<GlobalStats>();
        //note = GameObject.Find("LandCreationNote");
        
    }

    void Update()
    {
        if (creatingLand)
        {
            note.SetActive(true);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // get the collision point of the ray with the z = 0 plane
            Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int position = grid.WorldToCell(worldPoint);

            hoverTile.transform.position = position + hoverTileOffset;

            if (Input.GetMouseButton(0))
            {
                if(globalStats.GodForce > LandCost && LandTileMap.GetTile(position) == null) //create land on left click
                {
                    LandTileMap.SetTile(position, LandTile);  //remove water
                    WaterTileMap.SetTile(position, null); // create land

                    globalStats.GodForce -= LandCost; // update god force

                    //refresh colliders
                    WaterTileMap.GetComponent<CompositeCollider2D>().enabled = false;
                    WaterTileMap.GetComponent<CompositeCollider2D>().enabled = true; 

                    // update positions for pathfinder
                    position.x += 18;
                    position.y += 17;

                    //update pathfinder grid
                    var gg = AstarPath.active.data.gridGraph;
                    int x = position.x;
                    int y = position.y;
                    GridNodeBase node = gg.GetNode(x, y);

                    AstarPath.active.AddWorkItem(ctx => {
                        var grid = AstarPath.active.data.gridGraph;

                        // Mark a single node as unwalkable
                        grid.GetNode(x, y).Walkable = true;

                        // Recalculate the connections for that node as well as its neighbours
                        grid.CalculateConnectionsForCellAndNeighbours(x, y);
                    });
                }
            }

            if (Input.GetMouseButton(1)) // remove land on right click
            {
                if (LandTileMap.GetTile(position) != null)
                {
                    LandTileMap.SetTile(position, null); //remove land
                    WaterTileMap.SetTile(position, WaterTile); // return water

                    //refresh colliders
                    WaterTileMap.GetComponent<CompositeCollider2D>().enabled = false;
                    WaterTileMap.GetComponent<CompositeCollider2D>().enabled = true;

                    globalStats.GodForce += LandCost; // update god force

                    // update positions for pathfinder
                    position.x += 18;
                    position.y += 17;

                    //update pathfinder grid
                    var gg = AstarPath.active.data.gridGraph;
                    int x = position.x;
                    int y = position.y;
                    GridNodeBase node = gg.GetNode(x, y);

                    AstarPath.active.AddWorkItem(ctx => {
                        var grid = AstarPath.active.data.gridGraph;

                        // Mark a single node as unwalkable
                        grid.GetNode(x, y).Walkable = false;

                        // Recalculate the connections for that node as well as its neighbours
                        grid.CalculateConnectionsForCellAndNeighbours(x, y);
                    });
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                note.SetActive(false);
                hoverTile.transform.position = new Vector3(0f,-20f,0f);
                creatingLand = false;
            }
        }
    }

    public void CreateLandFunc()
    {
            creatingLand = true;
    }
}
