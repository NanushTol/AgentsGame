using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayValueBar : MonoBehaviour
{
    public FloatVariable BarValue;

    public float MaxValue;
    public float MinValue;

    public float BarMaxSize;
    public float BarMinSize;

    RectTransform _rect;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void UpdateBar()
    {
        float mapedBar = AgentUtils.Remap(BarValue.Value, MinValue, MaxValue, 0f, 120f);

        this._rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapedBar);
    }
}
