using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using WeatherSystem;

public struct CalculateWindJob : IJobParallelFor
{
    //public struct MediumCellsStruct
    //{
    //    public float[] Content;
    //    public int CellId;
    //    public Vector3Int GridPosition;
    //}

    //public struct WindCellsStruct
    //{
    //    public Vector2 MotionVector;
    //    public Vector2 RecivedMotionVector;
    //    public int CellId;
    //    public Vector3Int GridPosition;
    //}

    public NativeArray<MediumCellsStruct> MediumCellsReff;
    public NativeArray<MediumCellsStruct> TempCellsReturn;
    public NativeArray<WindCellsStruct> WindCellsReturn;


    public NativeArray<float> TempMaxVectorReturn;
    public NativeArray<float> MaxVector;

    public MediumCell[] MediumCells;
    public MediumCell[] TempCells;
    public WindCell[] WindCells;

    public bool DebugWind;
    public float ContentTransferRatio;
    public float HeatTransferRatio;
    public float Drag;

    public Vector2Int MapSize;

    public int[] AdjacentId;
    public float[] HeatDifference;
    public float[] DirectionPrecentage;
    public float[] DirectionAmplitude;
    public float TotalDifference;
    public float HorzTransfer;
    public float VertTransfer;

    //public float MaxVector;
    //public float TempMaxVector;

    public int DummyId;

    public void Execute(int index)
    {
        GetAdjacentCellsIds(index);

        // Calculate heat differences
        CalculateHeatDifference(index);

        // Calculate Direction & Amplitude
        CalculateDirectionAmplitude(index);

        // Calculate Direction Precentages
        CalculateDirectionPrecentage(index);

        CalculateMotionVector(index);

        if (DebugWind)
        {
            DrawDebugLine(index);

            CalculateMaxVector(index);
        }


        // returns
        MoveCellsContent(index);

        TransferWind(index);
    }



    public void GetAdjacentCellsIds(int cellId)
    {
        MediumCell cell = MediumCells[cellId];

        GetSideTiles(cell);
        GetTopBotCells(cell);
    }
    void GetSideTiles(MediumCell cell)
    {
        int cellId = cell.CellId;

        if (cell.GridPosition.x == -MapSize.x / 2)                 // if tile is on the Left edge
        {
            AdjacentId[1] = cellId + 1;                            // Right Cell ID
            AdjacentId[3] = cellId + MapSize.x - 1;               // Left Cell ID
        }
        else if (cell.GridPosition.x == (MapSize.x / 2) - 1)       // if tile is on the Right edge
        {
            AdjacentId[1] = cellId - (MapSize.x - 1);             // Right Cell ID
            AdjacentId[3] = cellId - 1;                            // Left Cell ID
        }
        else                                                        // else
        {
            AdjacentId[1] = cellId + 1;                            // Right Cell ID
            AdjacentId[3] = cellId - 1;                            // Left Cell ID
        }
    }
    void GetTopBotCells(MediumCell cell)
    {
        int cellId = cell.CellId;

        if (cell.GridPosition.y == -MapSize.y / 2)              // if tile is on the Buttom edge
        {
            AdjacentId[0] = cellId + MapSize.x;                //Top Cell ID 
            AdjacentId[2] = DummyId;                           // Buttom Cell ID = Dummy Cell
        }
        else if (cell.GridPosition.y == (MapSize.y / 2) - 1)    // if tile is on the Top edge
        {
            AdjacentId[0] = DummyId;                           // Top Cell ID = Dummy Cell
            AdjacentId[2] = cellId - MapSize.x;                // Buttom Cell ID
        }
        else                                                     // else
        {
            AdjacentId[0] = cellId + MapSize.x;                // Top Cell ID
            AdjacentId[2] = cellId - MapSize.x;                // Buttom Cell ID
        }
    }


    void CalculateHeatDifference(int cellId)
    {
        TotalDifference = 0f;

        for (int i = 0; i < 4; i++)
        {
            HeatDifference[i] = MediumCells[cellId].Content[4] - MediumCells[AdjacentId[i]].Content[4];

            TotalDifference += Math.Abs(HeatDifference[i]);
        }
    }

    void CalculateDirectionAmplitude(int cellId)
    {
        DirectionAmplitude[0] = HeatDifference[0] - HeatDifference[2]; // Up Down Direction
        DirectionAmplitude[1] = HeatDifference[1] - HeatDifference[3]; // Left Right Direction


        if (AdjacentId[2] == DummyId && DirectionAmplitude[0] > 0) // if Bottom adjacent cell is dummy
        {
            DirectionAmplitude[0] = 0;
        }
        else if (AdjacentId[0] == DummyId && DirectionAmplitude[0] < 0) // if Top adjacent cell is dummy
        {
            DirectionAmplitude[0] = 0;
        }
    }

    void CalculateDirectionPrecentage(int cellId)
    {
        DirectionPrecentage[0] = DirectionAmplitude[0] / TotalDifference;
        DirectionPrecentage[1] = DirectionAmplitude[1] / TotalDifference;

        for (int i = 0; i < 2; i++)
        {
            if (DirectionPrecentage[i] != DirectionPrecentage[i]) DirectionPrecentage[i] = 0; // if precentage = NaN
        }
    }

    void CalculateMotionVector(int cellId)
    {
        float differenceAvrage = TotalDifference / 4;

        Vector2 newV2 = new Vector2(DirectionPrecentage[1] * differenceAvrage, DirectionPrecentage[0] * differenceAvrage);

        WindCells[cellId].MotionVector = newV2 + WindCells[cellId].RecivedMotionVector;
        // job return
        var wndCellrtrn = WindCellsReturn[cellId];

        wndCellrtrn.MotionVectorX = WindCells[cellId].MotionVector.x;
        wndCellrtrn.MotionVectorY = WindCells[cellId].MotionVector.y;

        WindCellsReturn[cellId] = wndCellrtrn;


        WindCells[cellId].RecivedMotionVector = new Vector2(0f, 0f);
        // job return
        wndCellrtrn.RecivedMotionVectorX = 0f;
        wndCellrtrn.RecivedMotionVectorY = 0f;

        WindCellsReturn[cellId] = wndCellrtrn;
    }

    void DrawDebugLine(int cellId)
    {
        Vector3 cellCenter = new Vector3(WindCells[cellId].GridPosition.x + 0.5f, WindCells[cellId].GridPosition.y + 0.5f, 0f);

        float x = WindCells[cellId].MotionVector.x;
        float y = WindCells[cellId].MotionVector.y;

        x = Utils.Remap(x, 0f, MaxVector[0], 0f, 0.5f);
        y = Utils.Remap(y, 0f, MaxVector[0], 0f, 0.5f);

        Vector3 end = new Vector3(cellCenter.x + x, cellCenter.y + y, 0f);
        Vector3 dir = new Vector3(x, y, 0f);
        Vector3 head = Quaternion.AngleAxis(200, Vector3.forward) * dir * 0.5f;

        Debug.DrawLine(cellCenter, end, Color.red, 1f, false);
        Debug.DrawRay(end, head, Color.red, 1f);
    }

    void CalculateMaxVector(int cellId)
    {
        for (int i = 0; i < MediumCells.Length - 1; i++)
        {
            // job return
            if (WindCells[cellId].MotionVector.x > TempMaxVectorReturn[0]) TempMaxVectorReturn[0] = WindCells[cellId].MotionVector.x;
            if (WindCells[cellId].MotionVector.y > TempMaxVectorReturn[0]) TempMaxVectorReturn[0] = WindCells[cellId].MotionVector.y;
        }
    }

    void MoveCellsContent(int cellId)
    {

        // check give or recive by sign of the bigger: - recive, + give

        if (Math.Abs(HeatDifference[0]) > Math.Abs(HeatDifference[2])) // Interact with top cell
        {
            if (HeatDifference[0] > 0) // give
            {
                TransferContent(cellId, 0, -Math.Abs(WindCells[cellId].MotionVector.y));
            }
            else if (HeatDifference[0] < 0) // recive
            {
                TransferContent(cellId, 0, Math.Abs(WindCells[cellId].MotionVector.y));
            }
        }
        else if (Math.Abs(HeatDifference[0]) < Math.Abs(HeatDifference[2])) // interact with bottom cell
        {
            if (HeatDifference[2] > 0) // give
            {
                TransferContent(cellId, 2, -Math.Abs(WindCells[cellId].MotionVector.y));
            }
            else if (HeatDifference[2] < 0) // recive
            {
                TransferContent(cellId, 2, Math.Abs(WindCells[cellId].MotionVector.y));
            }
        }

        if (Math.Abs(HeatDifference[1]) > Math.Abs(HeatDifference[3])) // Interact with Right cell
        {
            if (HeatDifference[1] > 0) // give
            {
                TransferContent(cellId, 1, -Math.Abs(WindCells[cellId].MotionVector.x));
            }
            else if (HeatDifference[1] < 0) // recive
            {
                TransferContent(cellId, 1, Math.Abs(WindCells[cellId].MotionVector.x));
            }
        }
        else if (Math.Abs(HeatDifference[1]) < Math.Abs(HeatDifference[3])) // interact with Left cell
        {
            if (HeatDifference[3] > 0) // give
            {
                TransferContent(cellId, 3, -Math.Abs(WindCells[cellId].MotionVector.x));
            }
            else if (HeatDifference[3] < 0) // recive
            {
                TransferContent(cellId, 3, Math.Abs(WindCells[cellId].MotionVector.x));
            }
        }
    }

    void TransferContent(int cellId, int dir, float motionAxisValue)
    {
        MediumCell adjacentCell = MediumCells[AdjacentId[dir]];

        for (int c = 0; c < 5; c++)
        {
            if (c == 4)
            {
                // transfer heat
                HorzTransfer = motionAxisValue / adjacentCell.Content[c] * HeatTransferRatio;

                TempCells[cellId].Content[c] += HorzTransfer;
                TempCells[AdjacentId[dir]].Content[c] -= HorzTransfer;

                // job return
                var tmpCell = TempCellsReturn[cellId];
                var adjCell = TempCellsReturn[AdjacentId[dir]];

                tmpCell.Content4 = TempCells[cellId].Content[c];
                adjCell.Content4 = TempCells[AdjacentId[dir]].Content[c];

                TempCellsReturn[cellId] = tmpCell;
                TempCellsReturn[AdjacentId[dir]] = adjCell;
            }
            else
            {
                // transfer content
                HorzTransfer = (motionAxisValue * ContentTransferRatio) * adjacentCell.Content[c];

                if (HorzTransfer < adjacentCell.Content[c] * 4)
                {
                    TempCells[cellId].Content[c] += HorzTransfer;
                    TempCells[AdjacentId[dir]].Content[c] -= HorzTransfer;

                    // job return
                    var tmpCell = TempCellsReturn[cellId];
                    var adjCell = TempCellsReturn[AdjacentId[dir]];

                    if (c == 0)
                    {
                        tmpCell.Content0 = TempCells[cellId].Content[c];
                        adjCell.Content0 = TempCells[AdjacentId[dir]].Content[c];
                    }
                    else if (c == 1)
                    {
                        tmpCell.Content1 = TempCells[cellId].Content[c];
                        adjCell.Content1 = TempCells[AdjacentId[dir]].Content[c];
                    }
                    else if (c == 2)
                    {
                        tmpCell.Content2 = TempCells[cellId].Content[c];
                        adjCell.Content2 = TempCells[AdjacentId[dir]].Content[c];
                    }
                    else if (c == 3)
                    {
                        tmpCell.Content3 = TempCells[cellId].Content[c];
                        adjCell.Content3 = TempCells[AdjacentId[dir]].Content[c];
                    }

                    TempCellsReturn[cellId] = tmpCell;
                    TempCellsReturn[AdjacentId[dir]] = adjCell;
                }
            }
        }
    }

    void TransferWind(int cellId)
    {

        float ratio;
        Vector2 cellTransferRatio;

        Vector2 recivedV = WindCells[cellId].MotionVector - WindCells[cellId].MotionVector * Drag;

        if (Math.Abs(WindCells[cellId].MotionVector.x) > Math.Abs(WindCells[cellId].MotionVector.y) && WindCells[cellId].MotionVector.y != 0f)
        {
            ratio = Math.Abs(WindCells[cellId].MotionVector.y / WindCells[cellId].MotionVector.x);
            cellTransferRatio = new Vector2((0.5f * ratio) + (1f - ratio), (0.5f * ratio));
        }
        else if (Math.Abs(WindCells[cellId].MotionVector.x) < Math.Abs(WindCells[cellId].MotionVector.y) && WindCells[cellId].MotionVector.x != 0f)
        {
            ratio = Math.Abs(WindCells[cellId].MotionVector.x / WindCells[cellId].MotionVector.y);
            cellTransferRatio = new Vector2((0.5f * ratio), (0.5f * ratio) + (1f - ratio));
        }
        else cellTransferRatio = new Vector2(0.5f, 0.5f);

        //ratioV = new Vector2(0.5f, 0.5f);

        WindCellsStruct wndCellrtrn;

        if (WindCells[cellId].MotionVector.x > 0) // Right Movement
        {
            wndCellrtrn = WindCellsReturn[AdjacentId[1]];

            wndCellrtrn.RecivedMotionVectorX += recivedV.x * cellTransferRatio.x;
            wndCellrtrn.RecivedMotionVectorY += recivedV.y * cellTransferRatio.x;

            WindCellsReturn[AdjacentId[1]] = wndCellrtrn;
        }
        else if (WindCells[cellId].MotionVector.x < 0) // left Movement
        {
            wndCellrtrn = WindCellsReturn[AdjacentId[3]];

            wndCellrtrn.RecivedMotionVectorX += recivedV.x * cellTransferRatio.x;
            wndCellrtrn.RecivedMotionVectorY += recivedV.y * cellTransferRatio.x;

            WindCellsReturn[AdjacentId[3]] = wndCellrtrn;
        }

        if (WindCells[cellId].MotionVector.y > 0) // Up Movement
        {
            wndCellrtrn = WindCellsReturn[AdjacentId[0]];

            wndCellrtrn.RecivedMotionVectorX += recivedV.x * cellTransferRatio.x;
            wndCellrtrn.RecivedMotionVectorY += recivedV.y * cellTransferRatio.x;

            WindCellsReturn[AdjacentId[0]] = wndCellrtrn;
        }
        else if (WindCells[cellId].MotionVector.y < 0) // Down Movement
        {
            wndCellrtrn = WindCellsReturn[AdjacentId[2]];

            wndCellrtrn.RecivedMotionVectorX += recivedV.x * cellTransferRatio.x;
            wndCellrtrn.RecivedMotionVectorY += recivedV.y * cellTransferRatio.x;

            WindCellsReturn[AdjacentId[2]] = wndCellrtrn;
        }
    }
}


