﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public class Wind : MonoBehaviour
{
    public bool DebugWind = false;

    public float TotalHeat;

    public float ContentTransferRatio = 0.02f;
    public float HeatTransferRatio = 2f;


    public Tilemap WindTileMap;
    public TileBase WindTile;

    public int SelectedTile;

    Medium _airMedium;

    Vector2Int _mapSize;

    int[] _adjacentId = new int[4]; // Top, Right, Buttom, Left
    float[] _heatDifference = new float[4];
    float[] _directionPrecentage = new float[2];
    float[] _directionAmplitude = new float[2];
    float _totalDifference;
    float _directionAvrage;
    float _horzTransfer;
    float _vertTransfer;

    float _maxVector;
    float _tempMaxVector;



    WindCell[] _windCells;

    MediumCell[] _tempCells;

    int _counter;

    int _dummyId;

    public void Initialize()
    {
        _airMedium = GameObject.Find("MediumAir").GetComponent<Medium>();
        _mapSize = new Vector2Int(_airMedium.MapSize.x, _airMedium.MapSize.y);
        WindTileMap.ClearAllTiles();

        InitializeWindCells();
        InitializeTempCells();
        InitializeGraphics();

        _dummyId = _mapSize.x * _mapSize.y;
    }

    #region Initialization Functions
    void InitializeWindCells()
    {
        // Initialize WindCells acording to MediumCells
        _windCells = new WindCell[_airMedium.Cells.Length];
        for (int i = 0; i < _airMedium.Cells.Length; i++)
        {
            _windCells[i] = new WindCell(i, _airMedium.Cells[i].GridPosition);
        }
        ClearWindCells();
    }
    void InitializeTempCells()
    {
        // Initiate Temp MediumCells
        _tempCells = new MediumCell[_airMedium.Cells.Length];
        for (int i = 0; i < _airMedium.Cells.Length; i++)
        {
            _tempCells[i] = new MediumCell(i, _airMedium.Cells[i].GridPosition);
        }
        ClearTempCells();
    }
    void InitializeGraphics()
    {
        // Initializing Wind Graphics
        Color color = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < _airMedium.Cells.Length; i++)
        {
            WindTileMap.SetTile(_airMedium.Cells[i].GridPosition, WindTile);

            WindTileMap.SetTileFlags(_airMedium.Cells[i].GridPosition, TileFlags.None);

            WindTileMap.SetColor(_airMedium.Cells[i].GridPosition, color);
        }
    }

    void ClearWindCells()
    {
        for (int i = 0; i < _windCells.Length; i++)
        {
            for (int c = 0; c < 5; c++)
            {
                _windCells[i].MotionVector = new Vector2(0f, 0f);
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
    #endregion



    public void GetAdjacentCellsIds(int cellId)
    {
        MediumCell cell = _airMedium.Cells[cellId];

        GetSideTiles(cell);
        GetTopBotCells(cell);
    }

    void GetSideTiles(MediumCell cell)
    {
        int cellId = cell.CellId;

        if (cell.GridPosition.x == -_mapSize.x / 2)                 // if tile is on the Left edge
        {
            _adjacentId[1] = cellId + 1;                            // Right Cell ID
            _adjacentId[3] = cellId + _mapSize.x - 1;               // Left Cell ID
        }
        else if (cell.GridPosition.x == (_mapSize.x / 2) - 1)       // if tile is on the Right edge
        {
            _adjacentId[1] = cellId - (_mapSize.x - 1);             // Right Cell ID
            _adjacentId[3] = cellId - 1;                            // Left Cell ID
        }
        else                                                        // else
        {
            _adjacentId[1] = cellId + 1;                            // Right Cell ID
            _adjacentId[3] = cellId - 1;                            // Left Cell ID
        }
    }
    void GetTopBotCells(MediumCell cell)
    {
        int cellId = cell.CellId;

        if (cell.GridPosition.y == -_mapSize.y / 2)              // if tile is on the Buttom edge
        {
            _adjacentId[0] = cellId + _mapSize.x;                //Top Cell ID 
            _adjacentId[2] = _dummyId;                           // Buttom Cell ID = Dummy Cell
        }
        else if (cell.GridPosition.y == (_mapSize.y / 2) - 1)    // if tile is on the Top edge
        {
            _adjacentId[0] = _dummyId;                           // Top Cell ID = Dummy Cell
            _adjacentId[2] = cellId - _mapSize.x;                // Buttom Cell ID
        }
        else                                                     // else
        {
            _adjacentId[0] = cellId + _mapSize.x;                // Top Cell ID
            _adjacentId[2] = cellId - _mapSize.x;                // Buttom Cell ID
        }
    }

    void CalculateHeatDifference(int cellId)
    {
        _totalDifference = 0f;

        for (int i = 0; i < 4; i++)
        {
            _heatDifference[i] = _airMedium.Cells[cellId].Content[4] - _airMedium.Cells[_adjacentId[i]].Content[4];

            _totalDifference += Math.Abs(_heatDifference[i]);
        }
    }

    void CalculateDirectionAmplitude(int cellId)
    {
        _directionAmplitude[0] = _heatDifference[0] - _heatDifference[2]; // Up Down Direction
        _directionAmplitude[1] = _heatDifference[1] - _heatDifference[3]; // Left Right Direction


        if (_adjacentId[2] == _dummyId && _directionAmplitude[0] > 0) // if Bottom adjacent cell is dummy
        {
            _directionAmplitude[0] = 0;
        }
        else if (_adjacentId[0] == _dummyId && _directionAmplitude[0] < 0) // if Top adjacent cell is dummy
        {
            _directionAmplitude[0] = 0;
        }
    }

    void CalculateDirectionPrecentage(int cellId)
    {
        _directionPrecentage[0] = _directionAmplitude[0] / _totalDifference;
        _directionPrecentage[1] = _directionAmplitude[1] / _totalDifference;

        for (int i = 0; i < 2; i++)
        {
            if (_directionPrecentage[i] != _directionPrecentage[i]) _directionPrecentage[i] = 0; // if precentage = NaN
        }
    }

    void CalculateMotionVector(int cellId)
    {
        float differenceAvrage = _totalDifference / 4;

        _windCells[cellId].MotionVector = new Vector2(_directionPrecentage[1] * differenceAvrage, _directionPrecentage[0] * differenceAvrage);
    }

    void DrawDebugLine(int cellId)
    {
        Vector3 cellCenter = new Vector3(_windCells[cellId].GridPosition.x + 0.5f, _windCells[cellId].GridPosition.y + 0.5f, 0f);

        float x = _windCells[cellId].MotionVector.x;
        float y = _windCells[cellId].MotionVector.y;

        x = Utils.Remap(x, 0f, _maxVector, 0f, 0.5f);
        y = Utils.Remap(y, 0f, _maxVector, 0f, 0.5f);

        Vector3 end = new Vector3(cellCenter.x + x, cellCenter.y + y, 0f);
        Vector3 dir = new Vector3(x, y, 0f);
        Vector3 head = Quaternion.AngleAxis(200, Vector3.forward) * dir* 0.5f;

        Debug.DrawLine(cellCenter, end, Color.red, 1f, false);
        Debug.DrawRay(end, head, Color.red, 1f);
    }

    void CalculateMaxVector(int cellId)
    {
        for (int i = 0; i < _airMedium.Cells.Length - 1; i++)
        {
            if (_windCells[cellId].MotionVector.x > _tempMaxVector) _tempMaxVector = _windCells[cellId].MotionVector.x;
            if (_windCells[cellId].MotionVector.y > _tempMaxVector) _tempMaxVector = _windCells[cellId].MotionVector.y;
        }
    }


    void MoveCellsContent(int cellId) 
    {
        
        // check give or recive by sign of the bigger: - recive, + give

        if (Math.Abs(_heatDifference[0]) > Math.Abs(_heatDifference[2])) // Interact with top cell
        {
            if (_heatDifference[0] > 0) // give
            {
                TransferContent(cellId, 0, -Math.Abs(_windCells[cellId].MotionVector.y));
            }
            else if (_heatDifference[0] < 0) // recive
            {
                TransferContent(cellId, 0, Math.Abs(_windCells[cellId].MotionVector.y));
            }
        }
        else if(Math.Abs(_heatDifference[0]) < Math.Abs(_heatDifference[2])) // interact with bottom cell
        {
            if (_heatDifference[2] > 0) // give
            {
                TransferContent(cellId, 2, -Math.Abs(_windCells[cellId].MotionVector.y));
            }
            else if (_heatDifference[2] < 0) // recive
            {
                TransferContent(cellId, 2, Math.Abs(_windCells[cellId].MotionVector.y));
            }
        }

        if (Math.Abs(_heatDifference[1]) > Math.Abs(_heatDifference[3])) // Interact with Right cell
        {
            if (_heatDifference[1] > 0) // give
            {
                TransferContent(cellId, 1, -Math.Abs(_windCells[cellId].MotionVector.x));
            }
            else if (_heatDifference[1] < 0) // recive
            {
                TransferContent(cellId, 1, Math.Abs(_windCells[cellId].MotionVector.x));
            }
        }
        else if (Math.Abs(_heatDifference[1]) < Math.Abs(_heatDifference[3])) // interact with Left cell
        {
            if (_heatDifference[3] > 0) // give
            {
                TransferContent(cellId, 3, -Math.Abs(_windCells[cellId].MotionVector.x));
            }
            else if (_heatDifference[3] < 0) // recive
            {
                TransferContent(cellId, 3, Math.Abs(_windCells[cellId].MotionVector.x));
            }
        }

        //if (_windCells[cellId].MotionVector.x > 0) // Right Movement
        //{
        //    // Take Left cell
        //    TransferContent(cellId, 3, _windCells[cellId].MotionVector.x);
        //}
        //else if (_windCells[cellId].MotionVector.x < 0) // left Movement
        //{
        //    // Take Right cell
        //    TransferContent(cellId, 1, _windCells[cellId].MotionVector.x);
        //}

        //if (_windCells[cellId].MotionVector.y > 0) // Up Movement
        //{
        //    // Take Bottom cell
        //    TransferContent(cellId, 2, _windCells[cellId].MotionVector.y);
        //}
        //else if (_windCells[cellId].MotionVector.y < 0) // Down Movement
        //{
        //    // Take Top cell
        //    TransferContent(cellId, 0, _windCells[cellId].MotionVector.y);
        //}
    }


    void TransferContent(int cellId, int dir, float motionAxisValue)
    {
        MediumCell adjacentCell = _airMedium.Cells[_adjacentId[dir]];

        for (int c = 0; c < 5; c++)
        {
            if (c == 4)
            {
                _horzTransfer = motionAxisValue / adjacentCell.Content[c] * HeatTransferRatio;

                _tempCells[cellId].Content[c] += _horzTransfer;
                _tempCells[_adjacentId[dir]].Content[c] -= _horzTransfer;
            }
            else
            {
                _horzTransfer = (motionAxisValue * ContentTransferRatio) * adjacentCell.Content[c];

                if (_horzTransfer < adjacentCell.Content[c] * 4)
                {
                    _tempCells[cellId].Content[c] += _horzTransfer;
                    _tempCells[_adjacentId[dir]].Content[c] -= _horzTransfer;
                }
            }
        }
    }

    void ApplyCellsChanges()
    {
        for (int i = 0; i < _airMedium.Cells.Length - 1; i++)
        {
            for (int c = 0; c < 5; c++)
            {
                _airMedium.Cells[i].Content[c] = _tempCells[i].Content[c];
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
        //ClearTempCells();
        ResetTempCells();

        _tempMaxVector = 0f;

        for (int i = 0; i < _airMedium.Cells.Length - 1; i++)
        {
            // Get cell's neighbors
            GetAdjacentCellsIds(i);

            // Calculate heat differences
            CalculateHeatDifference(i);

            // Calculate Direction & Amplitude
            CalculateDirectionAmplitude(i);

            // Calculate Direction Precentages
            CalculateDirectionPrecentage(i);

            CalculateMotionVector(i);

            if (DebugWind)
            {
                DrawDebugLine(i);

                CalculateMaxVector(i);
            }

            MoveCellsContent(i);
        }

        ApplyCellsChanges();

        _maxVector = _tempMaxVector;

        if (DebugWind)
        {
            CalculateTotalHeat();
        }
    }
}
