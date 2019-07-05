using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetTimeScale : MonoBehaviour
{
    public TMP_InputField TimeScaleField;

    public void setTimeScale()
    {
        Time.timeScale = float.Parse(TimeScaleField.GetComponent<TMP_InputField>().text);
    }
}
