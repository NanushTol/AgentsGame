using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;



namespace WeatherSystem 
{
    public struct MediumCellsStruct
    {
        public float Content0;
        public float Content1;
        public float Content2;
        public float Content3;
        public float Content4;

        //public int CellId;
        //public Vector3Int GridPosition;
    }

    public struct WindCellsStruct
    {
        public float MotionVectorX;
        public float MotionVectorY;
        public float RecivedMotionVectorX;
        public float RecivedMotionVectorY;

        //public int CellId;
        //public Vector3Int GridPosition;
    }
}

