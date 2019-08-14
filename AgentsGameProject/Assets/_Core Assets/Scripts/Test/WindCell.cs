using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindCell
{
    public WindCell(int id, Vector3Int gridPosition)
    {
        CellId = id;
        GridPosition = gridPosition;
    }

    public Vector2 MotionVector;

    public int CellId;

    public Vector3Int GridPosition;
}
