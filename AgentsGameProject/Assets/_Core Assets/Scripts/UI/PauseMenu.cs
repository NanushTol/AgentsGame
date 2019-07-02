using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;

    float lastTimeScale;
    CreateLand createLand;

    private void Awake()
    {
        createLand = GameObject.Find("CreateLandButton").GetComponent<CreateLand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && createLand.creatingLand == false)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        
    }

    void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = lastTimeScale;
        GameIsPaused = false;
    }

    void Pause()
    {
        lastTimeScale = Time.timeScale;
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        GameIsPaused = true;
    }
}
