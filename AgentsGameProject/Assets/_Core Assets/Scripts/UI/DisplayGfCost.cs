using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayGfCost : MonoBehaviour
{
    public GameObject createAngel;
    public int GodForceCostIndex;

    TextMeshProUGUI textComponent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = createAngel.GetComponent<CreateAngel>().GodForceCosts[GodForceCostIndex].ToString("0.00");
    }
}
