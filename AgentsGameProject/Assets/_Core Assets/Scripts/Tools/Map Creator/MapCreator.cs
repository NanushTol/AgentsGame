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

    public Medium AirMedium;
    public TileBase MediumTile;


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

        AirMedium.InitializeMedium();
        CreateAirGraphics();


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

    

    void CreateAirGraphics()
    {
        AirMedium.InitializeGraphics();

        Color c = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < AirMedium.Cells.Length; j++)
            {
                AirMedium.TileMaps[i].SetTile(AirMedium.Cells[j].GridPosition, MediumTile);

                AirMedium.TileMaps[i].SetTileFlags(AirMedium.Cells[j].GridPosition, TileFlags.None);

                AirMedium.TileMaps[i].SetColor(AirMedium.Cells[j].GridPosition, c);
            }
        }

    }
}
