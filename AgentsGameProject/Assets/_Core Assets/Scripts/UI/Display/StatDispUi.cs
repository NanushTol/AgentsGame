using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDispUi : MonoBehaviour
{

    public GameObject globalStats;
    TextMeshProUGUI textComponent;
    public int statIndex;

    Dictionary<int, string> stats = new Dictionary<int, string>();

    private void Awake()
    {
        //textComponent = this.gameObject.GetComponent<TextMeshProUGUI>().text;
    }
    // Update is called once per frame
    void Update()
    {
        if (statIndex == 3 || statIndex == 4 || statIndex == 9 || statIndex == 10 || statIndex == 11 || statIndex == 12 || statIndex == 13)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = globalStats.GetComponent<GlobalStats>().Stats[statIndex].ToString("0.00");
        }
        else
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = globalStats.GetComponent<GlobalStats>().Stats[statIndex].ToString("0");
        }
        
    }
}
