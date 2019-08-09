using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

[CreateAssetMenu(menuName = "Adjactives Types/Producer Type")]
public class ProducerTypeSO : ScriptableObject
{
    [Header("Materials Per Tick")]
    public bool ProduceMaterials;
    public float Stone;
    public float Wood;
    public float Fuel;
    [HideInInspector]
    public float[] Materials = new float[3];

    [Header("Elements Per Tick")]
    public bool ProduceElements;
    public float Water;
    public float Co2;
    public float Oxygen;
    public float Nutrients;
    [HideInInspector]
    public float[] Elements = new float[4];

    [Header("Forces Per Tick")]
    public bool ProduceForces;
    public float Fire;
    public float Wind;
    public float Heat;
    [HideInInspector]
    public float[] Forces = new float[3];

    void OnEnable()
    {
        Materials[0] = Stone;
        Materials[1] = Wood;
        Materials[2] = Fuel;

        Elements[0] = Water;
        Elements[1] = Co2;
        Elements[2] = Oxygen;
        Elements[3] = Nutrients;

        Forces[0] = Fire;
        Forces[1] = Wind;
        Forces[2] = Heat;
    }
}
