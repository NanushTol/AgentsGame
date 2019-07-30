using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SleepPlace : MonoBehaviour
{

    public int SleepingAgents;

    TextMeshProUGUI amountUiElement;

    void Awake()
    {
        amountUiElement = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        amountUiElement.text = SleepingAgents.ToString();
    }

    void Update()
    {
        amountUiElement.text = SleepingAgents.ToString();
    }
}
