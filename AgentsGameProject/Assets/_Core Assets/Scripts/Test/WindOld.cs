using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public class WindOld : MonoBehaviour
{
    public float TotalHeat;

    public Tilemap WindTileMap;
    public TileBase WindTile;

    public int SelectedTile;

    Medium _airMedium;

    int[] _adjacentId = new int[4]; // Top, Right, Buttom, Left
    float[] _heatDifference = new float[4];
    float[] _directionPrecentage = new float[2];
    float[] _directionAmplitude = new float[2];
    float _differenceTotal;
    float _directionAvrage;

    MediumCell[] _adjacentCells = new MediumCell[4];

    MediumCell[] _tempCells;

    int _counter;

    int _dummyId;

    public void Initialize()
    {
        _airMedium = GameObject.Find("MediumAir").GetComponent<Medium>();

        Color color = new Color(1f, 1f, 1f, 1f);

        WindTileMap.ClearAllTiles();

        // Initiate TempGrid
        _tempCells = new MediumCell[_airMedium.Cells.Length];
        Vector3Int tempV3 = new Vector3Int(0, 0, 0);
        for (int i = 0; i < _airMedium.Cells.Length; i++)
        {
            _tempCells[i] = new MediumCell(i, tempV3);
        }
        ClearTempCells();

        // Initiate Adjacent Cells
        for (int a = 0; a < 4; a++)
        {
            _adjacentCells[a] = new MediumCell(a, tempV3);
        }
        ClearAdjacentCells();

        for (int i = 0; i < _airMedium.Cells.Length; i++)
        {
            WindTileMap.SetTile(_airMedium.Cells[i].GridPosition, WindTile);

            WindTileMap.SetTileFlags(_airMedium.Cells[i].GridPosition, TileFlags.None);

            WindTileMap.SetColor(_airMedium.Cells[i].GridPosition, color);
        }

        _dummyId = _airMedium.MapSize.x * _airMedium.MapSize.y;
    }


    public void GetAdjacentCellsIds(int cellId)
    {
        MediumCell cell = _airMedium.Cells[cellId];

        GetSideTiles(cell);
        GetTopBotCells(cell);
    }

    void GetSideTiles(MediumCell cell)
    {
        if (cell.GridPosition.x == -_airMedium.MapSize.x / 2)              // if tile is on the Left edge
        {
            _adjacentId[1] = cell.CellId + 1;                              // Right Cell ID
            _adjacentId[3] = cell.CellId + _airMedium.MapSize.x - 1;       // Left Cell ID
        }
        else if (cell.GridPosition.x == (_airMedium.MapSize.x / 2) - 1)    // if tile is on the Right edge
        {
            _adjacentId[1] = cell.CellId - (_airMedium.MapSize.x - 1);     // Right Cell ID
            _adjacentId[3] = cell.CellId - 1;                              // Left Cell ID
        }
        else                                                               // else
        {
            _adjacentId[1] = cell.CellId + 1;                              // Right Cell ID
            _adjacentId[3] = cell.CellId - 1;                              // Left Cell ID
        }
    }
    void GetTopBotCells(MediumCell cell)
    {
        if (cell.GridPosition.y == -_airMedium.MapSize.y / 2)               // if tile is on the Buttom edge
        {
            _adjacentId[0] = cell.CellId + _airMedium.MapSize.x;            //Top Cell ID 
            _adjacentId[2] = _dummyId;                                      // Buttom Cell ID = Dummy Cell
        }
        else if (cell.GridPosition.y == (_airMedium.MapSize.y / 2) - 1)     // if tile is on the Top edge
        {
            _adjacentId[0] = _dummyId;                                      // Top Cell ID = Dummy Cell
            _adjacentId[2] = cell.CellId - _airMedium.MapSize.x;            // Buttom Cell ID
        }
        else                                                                // else
        {
            _adjacentId[0] = cell.CellId + _airMedium.MapSize.x;            // Top Cell ID
            _adjacentId[2] = cell.CellId - _airMedium.MapSize.x;            // Buttom Cell ID
        }
    }

    void CalculateHeatDifference(int cellId)
    {
        _differenceTotal = 0f;

        for (int i = 0; i < 4; i++)
        {
            _heatDifference[i] = _airMedium.Cells[cellId].Content[4] - _airMedium.Cells[_adjacentId[i]].Content[4];

            if (_heatDifference[i] < 0)
            {
                _heatDifference[i] = 0f; // avoid calculating inward movment
            }

            _differenceTotal += _heatDifference[i];

            if(cellId == 405 || cellId == 435 || cellId == 464 || cellId == 465 || cellId == 466 || cellId == 495)
            {

            }
        }
    }

    void CalculateDirectionAmplitude()
    {
        _directionAmplitude[0] = _heatDifference[0] - _heatDifference[2]; // Up Down Direction
        _directionAmplitude[1] = _heatDifference[1] - _heatDifference[3]; // Left Right Direction

        

        if (_adjacentId[2] == _dummyId && _directionAmplitude[0] < 0) // if direction is Down & Bottom adjacent cell is dummy
        {
            _directionAmplitude[0] = -_directionAmplitude[0];
        }
        else if (_adjacentId[0] == _dummyId && _directionAmplitude[0] > 0) // if direction is Up & Top adjacent cell is dummy
        {
            _directionAmplitude[0] = -_directionAmplitude[0];
        }
    }

    void CalculateDirectionPrecentage()
    {
        float totalAmp = Math.Abs(_directionAmplitude[0]) + Math.Abs(_directionAmplitude[1]);

        _directionPrecentage[0] = _directionAmplitude[0] / totalAmp;
        _directionPrecentage[1] = _directionAmplitude[1] / totalAmp;

        if (_adjacentId[1] == 465)
        {

        }
    }

    void CalculateDirectionAvrage(int cellId)
    {
        float vert = 0f;
        float horz = 0f;

        // if direction is Up, get Top cell temp
        if (_directionAmplitude[0] > 0) vert = _airMedium.Cells[_adjacentId[0]].Content[4];
        // if direction is Down, get Bottom cell temp
        else if (_directionAmplitude[0] < 0) vert = _airMedium.Cells[_adjacentId[2]].Content[4];

        // if direction is Right, get Right cell temp
        if (_directionAmplitude[1] > 0) horz = _airMedium.Cells[_adjacentId[1]].Content[4];
        // if direction is Left, get Left cell temp
        else if (_directionAmplitude[1] < 0) horz = _airMedium.Cells[_adjacentId[3]].Content[4];

        if (vert == 0) vert = horz;
        if (horz == 0) horz = vert;
        _directionAvrage = (vert + horz) / 2;
    }

    void MoveCellContent(int cellId) 
    {
        float leftOver = _airMedium.Cells[cellId].Content[4] - _directionAvrage;

        for (int c = 0; c < 5; c++) // Cells Content
        {
            if (_directionPrecentage[0] > 0) // Move Up
            {
                _adjacentCells[0].Content[c] += Math.Abs(_directionPrecentage[0] * leftOver);
                _tempCells[cellId].Content[c] -= Math.Abs(_directionPrecentage[0] * leftOver);
            }
            else if (_directionPrecentage[0] < 0) // Move Down
            {
                _adjacentCells[2].Content[c] += Math.Abs(_directionPrecentage[0] * leftOver);
                _tempCells[cellId].Content[c] -= Math.Abs(_directionPrecentage[0] * leftOver);
            }
            else 
            {
                if (_differenceTotal > 0)
                {
                    _adjacentCells[0].Content[c] += _airMedium.Cells[cellId].Content[c] * 0.025f;
                    _adjacentCells[2].Content[c] += _airMedium.Cells[cellId].Content[c] * 0.025f;

                    _tempCells[cellId].Content[c] -= (_airMedium.Cells[cellId].Content[c] * 0.025f) * 2;
                }
            }


            if (_directionPrecentage[1] > 0) // Move Right
            {
                _adjacentCells[1].Content[c] += Math.Abs(_directionPrecentage[1] * leftOver);
                _tempCells[cellId].Content[c] -= Math.Abs(_directionPrecentage[1] * leftOver);
            }
            else if (_directionPrecentage[1] < 0) // Move Left
            {
                _adjacentCells[3].Content[c] += Math.Abs(_directionPrecentage[1] * leftOver);
                _tempCells[cellId].Content[c] -= Math.Abs(_directionPrecentage[1] * leftOver);
            }
            else
            {
                if (_differenceTotal > 0)
                {
                    _adjacentCells[1].Content[c] += _airMedium.Cells[cellId].Content[c] * 0.025f;
                    _adjacentCells[3].Content[c] += _airMedium.Cells[cellId].Content[c] * 0.025f;

                    _tempCells[cellId].Content[c] -= (_airMedium.Cells[cellId].Content[c] * 0.025f) * 2;
                }
            }

            if (cellId == 464)
            {

            }
        }
    }

    void StoreContentInTempArray()
    {
        for (int a = 0; a < 4; a++) // ajacent cells
        {
            for (int j = 0; j < 5; j++) // Content
            {
                _tempCells[_adjacentId[a]].Content[j] += _adjacentCells[a].Content[j];
            }
        }
    }

    void ClearTempCells()
    {
        for (int i = 0; i < _tempCells.Length; i++)
        {
            for (int c = 0; c < 5; c++)
            {
                _tempCells[i].Content[c] = 0f;
            }
        }
    }

    void ResetTempCells()
    {
        for (int i = 0; i < _airMedium.Cells.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                _tempCells[i].Content[j] = _airMedium.Cells[i].Content[j];
            }
        }
    }

    void ClearAdjacentCells()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int c = 0; c < 5; c++)
            {
                _adjacentCells[i].Content[c] = 0f;
            }
        }
    }

    void CalculateTotalHeat()
    {
        TotalHeat = 0f;
        for (int i = 0; i < _airMedium.Cells.Length - 1; i++)
        {
            TotalHeat += _airMedium.Cells[i].Content[4];
        }
    }

    public void CalculateWind()
    {
        // clear temp cells list
        ClearTempCells();
        //ResetTempCells();

        for (int i = 0; i < _airMedium.Cells.Length - 1; i++)
        {
            ClearAdjacentCells();

            // Get cell's neighbors
            GetAdjacentCellsIds(i);

            // Calculate heat differences
            CalculateHeatDifference(i);

            // Calculate Direction & Amplitude
            CalculateDirectionAmplitude();

            // Calculate Direction Precentages
            CalculateDirectionPrecentage();

            CalculateDirectionAvrage(i);

            // Move cell's content acording to precentage
            MoveCellContent(i);

            // Store new content in a temp cells list
            StoreContentInTempArray();
        }

        // Add new values to cells 
        for (int i = 0; i < _airMedium.Cells.Length - 1; i++)
        {
            for (int c = 0; c < 5; c++)
            {
                _airMedium.Cells[i].Content[c] += _tempCells[i].Content[c];
            }
        }

        CalculateTotalHeat();
        

        // Update neighbors
        //for (int a = 0; a < 4; a++) // ajacent cells
        //{
        //    for (int j = 0; j < 5; j++) // Content
        //    {
        //        _airMedium.Cells[_adjacentId[a]].Content[j] += _adjacentCells[a].Content[j];
        //    }
        //}
        //_counter += 1;


    }
}
