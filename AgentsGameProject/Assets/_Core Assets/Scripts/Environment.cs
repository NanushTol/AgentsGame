using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [Header("Stats")]
    public float Temperature;
    public float TempIn;
    public float TempOut;
    public float WorkTempIn;

    [Header("Variables")]
    public float WorkTempCost = 0.001f;

    public AnimationCurve HeatToEfficiency;

    public float HeatEfficiency;

    public GameObject ground;
    public Gradient gradient;
    public Color GroundTemperatureColor;

    void Update()
    {
        HeatEfficiency = HeatToEfficiency.Evaluate(Remap(Temperature, 0f, 38f, 0f, 1f));

        GroundTemperatureColor = gradient.Evaluate(Remap(Temperature, 0f, 38f, 0f, 1f));
        ground.GetComponent<Renderer>().material.color = GroundTemperatureColor;

        Temperature = Temperature + ((TempIn + WorkTempIn) - TempOut) * Time.deltaTime;
    }

    void LateUpdate()
    {
        WorkTempIn = 0f;
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        float _remapedValue = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        return _remapedValue;
    }

}
