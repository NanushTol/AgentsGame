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

        Color c = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < Cells.Length; j++)
            {
                TileMaps[i].SetTile(Cells[j].GridPosition, _mapCreator.MediumTile);

                TileMaps[i].SetTileFlags(Cells[j].GridPosition, TileFlags.None);

                TileMaps[i].SetColor(Cells[j].GridPosition, c);
            }
        }
    }

    public void InitializeMedium()
    {
        _mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();

        MapSize = new Vector2Int(_mapCreator.MapWidth, _mapCreator.MapHeight);

        Cells = new MediumCell[MapSize.x * MapSize.y + 1];

        int i = 0;
        for (int y = 0; y < MapSize.y; y++)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                Cells[i] = new MediumCell(i, new Vector3Int(x - (MapSize.x / 2), y - (MapSize.y / 2), 0));
                i++;
            }
        }

        //Cells[461].Content[4] = 21;
        //Cells[462].Content[4] = 22;

        //Cells[465].Content[1] = 2f;
        //Cells[465].Content[4] = 30f;


        //Cells[436].Content[4] = 25;

        //Cells[405].Content[4] = 30;
        //Cells[406].Content[4] = 25;
        //Cells[377].Content[4] = 20;

        //Cells[305].Content[4] = 30;


        //Dummy Cell
        Cells[MapSize.x * MapSize.y] = new MediumCell(MapSize.x * MapSize.y, new Vector3Int(-(MapSize.x / 2)-5,-(MapSize.y / 2)-5, 0));

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
        InitializeGraphics();
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= GraphicUpdateTime)
        {
            Wind.CalculateWind();
            UpdateGraphics();
            _elapsedTime = 0f;
        }

    }

    void UpdateGraphics()
    {
        for (int i = 0; i < TileMaps.Length; i++)
        {
            if (TileMaps[i].gameObject.activeInHierarchy)
            {
                for (int j = 0; j < Cells.Length; j++)
                {
                    float value = 0f;

                    if (i < 4)
                        value = Utils.Remap(Cells[j].Content[i], 0f, 10f, 0f, 1f);
                    else if (i == 4)
                        value = Utils.Remap(Cells[j].Content[i], 0f, 50f, 0f, 1f); //Heat Map

                    TileMaps[i].SetColor(Cells[j].GridPosition, MapsColor[i].Evaluate(value));
                }
            }
        }
    }
}
