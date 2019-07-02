using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelReset : MonoBehaviour
{
    public GlobalStats globalStats;
    // Update is called once per frame
    void Update()
    {
       if(globalStats.GetComponent<GlobalStats>().Population <= 0)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("level", 0);
        }
    }
}
