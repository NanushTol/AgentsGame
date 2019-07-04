using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.PostProcessing;


public class Environment : MonoBehaviour
{
    [Header("Stats")]
    public float Temperature;
    public float TempIn;
    public float TempOut;
    public float HeatEfficiency;

    [HideInInspector]
    public float WorkTempIn;

    [HideInInspector]
    public Color GroundTemperatureColor;

    [Header("Variables")]
    public float WorkTempCost = 0.001f;

    public AnimationCurve HeatToEfficiency;

    public Gradient gradient;
    
    


    PostProcessVolume ppVolume;
    Vignette ppVignette;


    void Start()
    {
        //ppVignette = ScriptableObject.CreateInstance<Vignette>();
        PostProcessVolume volume = GameObject.Find("PostProcessing").GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out ppVignette);

        //ppVignette = GameObject.Find("PostProcessing").GetComponent<PostProcessVolume>().profile.GetComponent<Vignette>();
        ppVignette.enabled.Override(true);
        ppVignette.intensity.Override(0f);
        ppVignette.color.Override(Color.white);

        //ppVolume = PostProcessManager.instance.QuickVolume(LayerMask.GetMask("PostProcess"), 100f, ppVignette);
    }

    void Update()
    {
        HeatEfficiency = HeatToEfficiency.Evaluate(Remap(Temperature, 0f, 38f, 0f, 1f));

        GroundTemperatureColor = gradient.Evaluate(Remap(Temperature, 0f, 38f, 0f, 1f));

        Temperature = Temperature + ((TempIn + WorkTempIn) - TempOut) * Time.deltaTime;

        ppVignette.color.value = GroundTemperatureColor;
        ppVignette.intensity.value = Remap(HeatEfficiency, 1f, 0f, 0f, 0.42f);
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
