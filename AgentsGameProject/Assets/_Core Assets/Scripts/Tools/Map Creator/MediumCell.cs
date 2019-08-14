using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumCell
{
    public MediumCell(int id, Vector3Int gridPosition)
    {
        CellId = id;
        GridPosition = gridPosition;
    }

    public float[] Content = new float[] { 0f, 0f, 4f, 0f, 20f }; // {Water, Co2, Oxy, Nutrients, Heat }

    public int CellId;

    public Vector3Int GridPosition;
}
