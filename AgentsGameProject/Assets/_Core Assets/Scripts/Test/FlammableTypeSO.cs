using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Adjactives Types/Flammable Type")][System.Serializable]
public class FlammableTypeSO : ScriptableObject
{
    //public float[] IO = new float[5];

    [System.Serializable]
    public struct IOStruct
    {
        public string Name;
        public float ValuePerTick;

        public IOStruct(string name, float value)
        {
            Name = name;
            ValuePerTick = value;
        }
    }

    public IOStruct[] IO = new IOStruct[]
    {
        new IOStruct("Water", 0f ),
        new IOStruct("Co2", 0f ),
        new IOStruct("Oxigen", 0f ),
        new IOStruct("Nutrients", 0f ),
        new IOStruct("Heat", 0f )
    };
}
