using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Pathfinding;

[ExecuteInEditMode]
public class MapCreator : MonoBehaviour
{

    public int MapWidth;
    public int MapHeight;


    public int LandWidth;
    public int LandHeight;


    public Tilemap LandTileMap;
    public Tilemap WaterTileMap;
    public TileBase LandTile;
    public TileBase WaterTile;

    Vector2Int mapCenter;
    Vector2Int landOffset;

    Vector2Int PfPosition;

    public void CreateMap()
    {
        LandTileMap.ClearAllTiles();
        WaterTileMap.ClearAllTiles();

        mapCenter = new Vector2Int(MapWidth / 2, MapHeight / 2);

        // Create Map
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                Vector3Int position = new Vector3Int(x - mapCenter.x, y - mapCenter.y, 0);
                WaterTileMap.SetTile(position, WaterTile);
            }
        }

        // Create Land
        
        landOffset = new Vector2Int(mapCenter.x - (LandWidth / 2), mapCenter.y - (LandHeight / 2));

        for (int x = 0; x < LandWidth; x++)
        {
            for (int y = 0; y < LandHeight; y++)
            {
                Vector3Int position = new Vector3Int(x + landOffset.x - mapCenter.x, y + landOffset.y - mapCenter.y, 0);
                LandTileMap.SetTile(position, LandTile);
                WaterTileMap.SetTile(position, null);
            }
        }
    }
}
