using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using static Constants;

public class Medium : MonoBehaviour
{
    public MediumTypes Type;

    public float GraphicUpdateTime;
    float _elapsedTime;

    public Wind Wind;

    public MediumCell[] Cells;
    public Tilemap[] TileMaps;

    public Gradient[] MapsColor = new Gradient[5];


    [HideInInspector]
    public Vector2Int MapSize;

    MapCreator _mapCreator;

    [HideInInspector]
    public List<MediumCell> CellsToUpdate = new List<MediumCell>();

    public void InitializeGraphics()
    {
        TileMaps = new Tilemap[5];

        for (int i = 0; i < TileMaps.Length; i++)
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
        for (int y = 0; y < MapSize.y; y++)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                Cells[i] = new MediumCell(i, new Vector3Int(x - (MapSize.x / 2), y - (MapSize.y / 2), 0));
                i++;
            }
        }

        Wind.Initialize();
    }


    public MediumCell GetCellByPosition(Vector2Int position)
    {
        Vector2Int mapCenter = new Vector2Int(MapSize.x / 2, MapSize.y / 2);

        int index = MapSize.x * (position.y + mapCenter.y) + (position.x + mapCenter.x);

        return Cells[index];
    }

    void Awake()
    {
        InitializeMedium();    
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= GraphicUpdateTime)
        {
            UpdateGraphics();
            _elapsedTime = 0f;
        }

    }

    void UpdateGraphics()
    {
        for (int i = 0; i < TileMaps.Length; i++)
        {
            for (int j = 0; j < Cells.Length; j++)
            {
                float value = 0f; ;
                if (i < 4)
                    value = Utils.Remap(Cells[j].Content[i], 0f, 2f, 0f, 1f);
                else if (i == 4)
                    value = Utils.Remap(Cells[j].Content[i], 0f, 50f, 0f, 1f); //Heat Map

                TileMaps[i].SetColor(Cells[j].GridPosition, MapsColor[i].Evaluate(value));
            }
        }
    }
}
