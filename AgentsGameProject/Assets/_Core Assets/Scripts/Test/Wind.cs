using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Collections;
using System.Linq;
using System;
using WeatherSystem;
using Unity.Jobs;

public class Wind : MonoBehaviour
{
    public bool DebugWind = false;

    public float TotalHeat;

    public float ContentTransferRatio = 0.02f;
    public float HeatTransferRatio = 2f;
    public float Drag = 2f;

    public Tilemap WindTileMap;
    public TileBase WindTile;

    public int SelectedTile;

    Medium _airMedium;

    MediumCell[] _mediumCells;
    MediumCell[] _tempCells;
    WindCell[] _windCells;

    Vector2Int _mapSize;

    int[] _adjacentId = new int[4]; // Top, Right, Buttom, Left
    float[] _heatDifference = new float[4];
    float[] _directionPrecentage = new float[2];
    float[] _directionAmplitude = new float[2];
    float _totalDifference;
    float _horzTransfer;
    float _vertTransfer;

    float _maxVector;
    float _tempMaxVector;

    int _dummyId;

    JobHandle jobHandle;
    CalculateWindJob CalcWindJob;

    public void Initialize()
    {
        _airMedium = GameObject.Find("MediumAir").GetComponent<Medium>();
        _mapSize = new Vector2Int(_airMedium.MapSize.x, _airMedium.MapSize.y);
        WindTileMap.ClearAllTiles();

        InitializeMediumCells();
        InitializeWindCells();
        InitializeTempCells();
        InitializeGraphics();

        _dummyId = _mapSize.x * _mapSize.y;
    }

    public void CalculateWind()
    {
        // clear temp cells list
        ResetCells(0);
        ResetCells(1);

        _tempMaxVector = 0f;

        for (int i = 0; i < _mediumCells.Length - 1; i++)
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

            TransferWind(i);
        }

        ApplyCellsChanges();

        _maxVector = _tempMaxVector;

        if (DebugWind)
        {
            CalculateTotalHeat();
        }
    }

    public void CalculateWindJobFunc()
    {
        NativeArray<MediumCellsStruct> mediumCellsReff = new NativeArray<MediumCellsStruct>(_mediumCells.Length, Allocator.TempJob);
        NativeArray<MediumCellsStruct> tempCellsReturn = new NativeArray<MediumCellsStruct>(_mediumCells.Length, Allocator.TempJob);
        NativeArray<WindCellsStruct> windCellsReturn = new NativeArray<WindCellsStruct>(_windCells.Length, Allocator.TempJob);
        NativeArray<float> tempMaxVectorReturn = new NativeArray<float>(1, Allocator.TempJob);
        NativeArray<float> maxVector = new NativeArray<float>(1, Allocator.TempJob);

        // clear temp cells list
        ResetCells(0);
        ResetCells(1);

        _tempMaxVector = 0f;

        // Set up the job data
        InitializeJob(mediumCellsReff, tempCellsReturn, windCellsReturn, maxVector, tempMaxVectorReturn);

        // Schedule the job
        jobHandle = CalcWindJob.Schedule(mediumCellsReff.Length, 1);

        // Wait for the job to complete
        jobHandle.Complete();

        // Getting results
        GetJobResults(tempCellsReturn, windCellsReturn, tempMaxVectorReturn);

        ApplyCellsChanges();

        _maxVector = _tempMaxVector;

        if (DebugWind)
        {
            CalculateTotalHeat();
        }

        // Free the memory allocated by the result array
        mediumCellsReff.Dispose();
        tempCellsReturn.Dispose();
        windCellsReturn.Dispose();
        maxVector.Dispose();
        tempMaxVectorReturn.Dispose();
    }

    private void GetJobResults(NativeArray<MediumCellsStruct> tempCellsReturn, NativeArray<WindCellsStruct> windCellsReturn, NativeArray<float> tempMaxVectorReturn)
    {
        for (int i = 0; i < _mediumCells.Length; i++)
        {
            _tempCells[i].Content[0] = tempCellsReturn[i].Content0;
            _tempCells[i].Content[1] = tempCellsReturn[i].Content1;
            _tempCells[i].Content[2] = tempCellsReturn[i].Content2;
            _tempCells[i].Content[3] = tempCellsReturn[i].Content3;
            _tempCells[i].Content[4] = tempCellsReturn[i].Content4;


            _windCells[i].MotionVector.x = windCellsReturn[i].MotionVectorX;
            _windCells[i].MotionVector.y = windCellsReturn[i].MotionVectorY;
            _windCells[i].RecivedMotionVector.x = windCellsReturn[i].RecivedMotionVectorX;
            _windCells[i].RecivedMotionVector.y = windCellsReturn[i].RecivedMotionVectorY;
        }
        _tempMaxVector = tempMaxVectorReturn[0];
    }

    private void InitializeJob(NativeArray<MediumCellsStruct> mediumCellsReff, NativeArray<MediumCellsStruct> tempCellsReturn, 
        NativeArray<WindCellsStruct> windCellsReturn, NativeArray<float> maxVector, NativeArray<float> tempMaxVectorReturn)
    {
        for (int i = 0; i < _mediumCells.Length; i++)
        {
            mediumCellsReff[i] = new MediumCellsStruct
            {
                Content0 = _mediumCells[i].Content[0],
                Content1 = _mediumCells[i].Content[1],
                Content2 = _mediumCells[i].Content[2],
                Content3 = _mediumCells[i].Content[3],
                Content4 = _mediumCells[i].Content[4]
            };
            tempCellsReturn[i] = new MediumCellsStruct
            {
                Content0 = _mediumCells[i].Content[0],
                Content1 = _mediumCells[i].Content[1],
                Content2 = _mediumCells[i].Content[2],
                Content3 = _mediumCells[i].Content[3],
                Content4 = _mediumCells[i].Content[4]
            };

            windCellsReturn[i] = new WindCellsStruct
            {
                MotionVectorX = _windCells[i].MotionVector.x,
                MotionVectorY = _windCells[i].MotionVector.y,
                RecivedMotionVectorX = _windCells[i].RecivedMotionVector.x,
                RecivedMotionVectorY = _windCells[i].RecivedMotionVector.y
            };
        }
        maxVector[0] = _maxVector;
        tempMaxVectorReturn[0] = _tempMaxVector;


        CalcWindJob = new CalculateWindJob();

        CalcWindJob.MediumCellsReff = mediumCellsReff;
        CalcWindJob.TempCellsReturn = tempCellsReturn;
        CalcWindJob.WindCellsReturn = windCellsReturn;

        CalcWindJob.MediumCells = _mediumCells;

        CalcWindJob.TempCells = _tempCells;
        CalcWindJob.WindCells = _windCells;

        CalcWindJob.DebugWind = DebugWind;
        CalcWindJob.ContentTransferRatio = ContentTransferRatio;
        CalcWindJob.HeatTransferRatio = HeatTransferRatio;
        CalcWindJob.Drag = Drag;

        CalcWindJob.MapSize = _mapSize;

        CalcWindJob.AdjacentId = _adjacentId;
        CalcWindJob.HeatDifference = _heatDifference;
        CalcWindJob.DirectionPrecentage = _directionPrecentage;
        CalcWindJob.DirectionAmplitude = _directionAmplitude;
        CalcWindJob.TotalDifference = _totalDifference;
        CalcWindJob.HorzTransfer = _horzTransfer;
        CalcWindJob.VertTransfer = _vertTransfer;

        CalcWindJob.MaxVector = maxVector;
        CalcWindJob.TempMaxVectorReturn = tempMaxVectorReturn;

        CalcWindJob.DummyId = _dummyId;
    }




    #region Initialization Functions
    void InitializeMediumCells()
    {
        // Initiate Temp MediumCells
        _mediumCells = new MediumCell[_airMedium.Cells.Length];
        for (int i = 0; i < _airMedium.Cells.Length; i++)
        {
            _mediumCells[i] = new MediumCell(i, _airMedium.Cells[i].GridPosition);
        }
        ClealCells(1);
    }
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
        ClealCells(0);
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
    /// <summary>
    /// int type: 0 = _tempCells | 1 = _mediumCells
    /// </summary>
    void ClealCells(int type)
    {
        if (type == 0)
        {
            for (int i = 0; i < _tempCells.Length; i++)
            {
                for (int c = 0; c < 5; c++)
                {
                    _tempCells[i].Content[c] = 0f;
                }
            }
        }
        else if (type == 1)
        {
            for (int i = 0; i < _mediumCells.Length; i++)
            {
                for (int c = 0; c < 5; c++)
                {
                    _mediumCells[i].Content[c] = 0f;
                }
            }
        }
        
    }
    /// <summary>
    /// int type: 0 = _tempCells | 1 = _mediumCells
    /// </summary>
    void ResetCells(int type)
    {
        if (type == 0)
        {
            for (int i = 0; i < _airMedium.Cells.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _tempCells[i].Content[j] = _airMedium.Cells[i].Content[j];
                }
            }
        }
        else if (type == 1)
        {
            for (int i = 0; i < _airMedium.Cells.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _mediumCells[i].Content[j] = _airMedium.Cells[i].Content[j];
                }
            }
        }
    }
    #endregion

    #region Job Functions

    public void GetAdjacentCellsIds(int cellId)
    {
        MediumCell cell = _mediumCells[cellId];

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
            _heatDifference[i] = _mediumCells[cellId].Content[4] - _mediumCells[_adjacentId[i]].Content[4];

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

        if (cellId == 585)
        {

        }
        if (cellId == 615)
        {

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

        Vector2 newV2 = new Vector2(_directionPrecentage[1] * differenceAvrage, _directionPrecentage[0] * differenceAvrage);

        _windCells[cellId].MotionVector = newV2 + _windCells[cellId].RecivedMotionVector;
        _windCells[cellId].RecivedMotionVector = new Vector2(0f, 0f);

        
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
        for (int i = 0; i < _mediumCells.Length - 1; i++)
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
    }

    void TransferContent(int cellId, int dir, float motionAxisValue)
    {
        MediumCell adjacentCell = _mediumCells[_adjacentId[dir]];

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

    void TransferWind(int cellId)
    {
        
        float ratio;
        Vector2 cellTransferRatio;

        Vector2 recivedV = _windCells[cellId].MotionVector - _windCells[cellId].MotionVector * Drag;

        if (Math.Abs(_windCells[cellId].MotionVector.x) > Math.Abs(_windCells[cellId].MotionVector.y) && _windCells[cellId].MotionVector.y != 0f)
        {
            ratio = Math.Abs(_windCells[cellId].MotionVector.y / _windCells[cellId].MotionVector.x);
            cellTransferRatio = new Vector2((0.5f * ratio) + (1f - ratio), (0.5f * ratio));
        }
        else if (Math.Abs(_windCells[cellId].MotionVector.x) < Math.Abs(_windCells[cellId].MotionVector.y) && _windCells[cellId].MotionVector.x != 0f)
        {
            ratio = Math.Abs(_windCells[cellId].MotionVector.x / _windCells[cellId].MotionVector.y);
            cellTransferRatio = new Vector2((0.5f * ratio), (0.5f * ratio) + (1f - ratio));
        }
        else cellTransferRatio = new Vector2(0.5f, 0.5f);


        if (_windCells[cellId].MotionVector.x > 0) // Right Movement
        {
            _windCells[_adjacentId[1]].RecivedMotionVector +=  recivedV * cellTransferRatio.x;
        }
        else if (_windCells[cellId].MotionVector.x < 0) // left Movement
        {
            _windCells[_adjacentId[3]].RecivedMotionVector +=  recivedV * cellTransferRatio.x;
        }

        if (_windCells[cellId].MotionVector.y > 0) // Up Movement
        {
            _windCells[_adjacentId[0]].RecivedMotionVector +=  recivedV * cellTransferRatio.y;
        }
        else if (_windCells[cellId].MotionVector.y < 0) // Down Movement
        {
            _windCells[_adjacentId[2]].RecivedMotionVector +=  recivedV * cellTransferRatio.y;
        }

        if (cellId == 465)
        {

        }
    }
    #endregion

    #region Class Functions
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
    #endregion


}



