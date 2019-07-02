using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using Pathfinding;

public class CreateLand : MonoBehaviour
{
    public Grid grid;
    public Tilemap LandTileMap;
    public Tilemap WaterTileMap;
    public TileBase LandTile;
    public TileBase WaterTile;

    GameObject hoverTile;
    Vector3 hoverTileOffset = new Vector3(0.5f, 0.5f, 0f);

    [HideInInspector]
    public bool creatingLand;

    private void Awake()
    {
        hoverTile = GameObject.Find("HoverTile");
    }

    void Update()
    {
        if (creatingLand)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // get the collision point of the ray with the z = 0 plane
            Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int position = grid.WorldToCell(worldPoint);

            hoverTile.transform.position = position + hoverTileOffset;

            if (Input.GetMouseButton(0))
            {
                LandTileMap.SetTile(position, LandTile);
                WaterTileMap.SetTile(position, null);

                position.x += 18;
                position.y += 17;

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

            if (Input.GetMouseButton(1))
            {
                
                LandTileMap.SetTile(position, null);
                WaterTileMap.SetTile(position, WaterTile);

                position.x += 18;
                position.y += 17;

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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
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
