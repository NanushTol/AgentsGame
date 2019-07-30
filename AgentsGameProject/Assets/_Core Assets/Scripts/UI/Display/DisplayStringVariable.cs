using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayStringVariable : MonoBehaviour
{

    public StringVariable StringToDisplay;

    TextMeshProUGUI textElement;

    private void Awake()
    {
        textElement = this.GetComponent<TextMeshProUGUI>();
        textElement.text = "Text";
    }

    public void UpdateTextValue()
    {
        textElement.text = StringToDisplay.Value;
    }
}
