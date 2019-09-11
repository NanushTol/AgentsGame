using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherSystem
{
    public class WindCell
    {
        public WindCell(int id, Vector3Int gridPosition)
        {
            CellId = id;
            GridPosition = gridPosition;
        }

        public Vector2 MotionVector;
        public Vector2 RecivedMotionVector;

        public int CellId;

        public Vector3Int GridPosition;
    }
}

