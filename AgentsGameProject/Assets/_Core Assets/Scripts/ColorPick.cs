using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorPick : MonoBehaviour
{
    public GameObject ColorPanel;
    public GameObject ColorButton;
    public Color[] colors;
    public int ColorId;


    // Start is called before the first frame update
    void Awake()
    {
        /*
        colors = new Color[5];
        colors[0] = new Color(0.960f, 0.278f, 0.447f, 1f);
        colors[1] = new Color(1f, 0.569f, 0.183f, 1f);
        colors[2] = new Color(0.278f, 0.869f, 0.960f, 1f);
        colors[3] = new Color(0.645f, 0.278f, 0.960f, 1f);
        colors[4] = new Color(1f, 0.897f, 0.127f, 1f);
        */
    }


    public void PickColorClick()
    {
        ColorButton.GetComponent<Image>().color = colors[ColorId];
        //Debug.Log("color id: " + ColorId);
        ColorPanel.SetActive(false);
    }

}
