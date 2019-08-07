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

    //public float WaterContent;
    //public float Co2Content;
    //public float NutrientsContent;
    //public float OxygenContent;
    //public float HeatContent;

    public float[] Content = new float[] { 0f,0f,0f,0f,0f };

    public int CellId;

    public Vector3Int GridPosition;
    public Vector2 WorldPosition;

}
