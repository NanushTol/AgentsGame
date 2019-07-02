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

        if (float.Parse(TimeScaleField.GetComponent<TMP_InputField>().text) == 0f)
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = float.Parse(TimeScaleField.GetComponent<TMP_InputField>().text);
        }
    }
}
