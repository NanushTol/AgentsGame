using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Wind : MonoBehaviour
{


    public Tilemap WindTileMap;
    public TileBase WindTile;

    public int SelectedTile;

    Medium _airMedium;

    int[] _adjacentCellsId = new int[4]; // Top, Right, Buttom, Left
    MediumCell[] _adjacentCells = new MediumCell[4];

    public void Initialize()
    {
        _airMedium = GameObject.Find("MediumAir").GetComponent<Medium>();

        Color c = new Color(1f, 1f, 1f, 1f);

        WindTileMap.ClearAllTiles();

        for (int i = 0; i < _airMedium.Cells.Length; i++)
        {
            if (i == 2495)
            {
            }

            WindTileMap.SetTile(_airMedium.Cells[i].GridPosition, WindTile);

            WindTileMap.SetTileFlags(_airMedium.Cells[i].GridPosition, TileFlags.None);

            WindTileMap.SetColor(_airMedium.Cells[i].GridPosition, c);
        }
    }


    public void GetAdjacentCells()
    {
        MediumCell cell = _airMedium.Cells[SelectedTile];

        _adjacentCellsId[0] = cell.CellId + _airMedium.MapSize.x;       // Top Cell ID

        if (SelectedTile % 100 == 99)                                   // Right Cell ID
            _adjacentCellsId[1] = cell.CellId - 99;
        else _adjacentCellsId[1] = cell.CellId + 1;

        _adjacentCellsId[2] = cell.CellId - _airMedium.MapSize.x;       // Buttom Cell ID

        if(SelectedTile % 100 == 0)                                     // Left Cell ID
            _adjacentCellsId[3] = cell.CellId + 99;
        else _adjacentCellsId[3] = cell.CellId - 1;


        WindTileMap.SetColor(cell.GridPosition, Color.blue);

        for (int i = 0; i < 4; i++)
        {
            _adjacentCells[i] = _airMedium.Cells[_adjacentCellsId[i]];
            WindTileMap.SetColor(_adjacentCells[i].GridPosition, Color.red);
        }
    }
}
