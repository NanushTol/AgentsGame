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

    public void Initialize(MaterialTypes material)
    {
        MaterialType = material;
    }
}

