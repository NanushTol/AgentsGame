using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class GameMaterial : MonoBehaviour
{
    //[HideInInspector]
    public MaterialTypes MaterialType;
    //[HideInInspector]
    public float Amount;
    //[HideInInspector]
    public bool Wet = false;

    public void Initialize(MaterialTypes material)
    {
        MaterialType = material;
    }
}

