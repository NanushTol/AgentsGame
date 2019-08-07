using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Element
{
    public int ElementType { get; }
    public float Value { get; set; }

    public void SetValue(float value)
    {
        Value = value;
    }
}
