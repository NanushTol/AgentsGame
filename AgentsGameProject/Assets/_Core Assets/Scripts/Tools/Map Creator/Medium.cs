using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using static Constants;

public class Medium : MonoBehaviour
{
    public MediumTypes Type;

    public MediumCell[] Cells;

    public Tilemap[] TileMaps;

    [HideInInspector]
    public Vector2Int MapSize;

    MapCreator _mapCreator;

    public void InitializeGraphics()
    {
        TileMaps = new Tilemap[5];

        for (int i = 0; i < 5; i++)
        {
            TileMaps[i] = transform.GetChild(i).GetComponent<Tilemap>();
            TileMaps[i].ClearAllTiles();
        }
    }

    public void InitializeMedium()
    {
        _mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();

        MapSize = new Vector2Int(_mapCreator.MapWidth, _mapCreator.MapHeight);

        Cells = new MediumCell[MapSize.x * MapSize.y];

        int i = 0;
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Cells[i] = new MediumCell(i, new Vector3Int(x - (MapSize.x / 2), y - (MapSize.y / 2), 0));
                i++;
            }
        }
    }


    public MediumCell GetCellByPosition(Vector2Int position)
    {
        Vector2Int mapCenter = new Vector2Int(MapSize.x / 2, MapSize.y / 2);

        int index = MapSize.x * (position.x + mapCenter.x) + (position.y + mapCenter.y);

        return Cells[index];
    }

    void Awake()
    {
        InitializeMedium();    
    }

    void Update()
    {
        for (int i = 0; i < TileMaps.Length; i++)
        {
            for (int j = 0; j < Cells.Length; j++)
            {
                TileMaps[i].SetColor(Cells[j].GridPosition, new Color(1f, 0f, 0f, Cells[j].Content[i]));
            }
        }
    }
}
