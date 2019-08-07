using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;


public class Wood : GameMaterial
{
    Flammable _flammable;
    FlammableTypeSO _flammableType;

    void Awake()
    {
        Initialize(MaterialTypes.Wood);
        _flammable = gameObject.AddComponent<Flammable>();
        _flammableType = Resources.Load<FlammableTypeSO>("FlammableWood");
        _flammable.Initialize(this, _flammableType);
    }
}
