using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayFloatVariable : MonoBehaviour
{

    public FloatVariable VariableToDisplay;

    public string StringFormat;

    TextMeshProUGUI textElement;

    private void Awake()
    {
        textElement = this.GetComponent<TextMeshProUGUI>();
        textElement.text = "VariableToDisplay";
    }

    public void UpdateTextValue()
    {
        textElement.text = VariableToDisplay.Value.ToString(StringFormat);
    }
}
