using UnityEngine;

public class UserControls : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject TimeNote;

    GameStates gameStates;
    float lastTimeScale;
   
    void Awake()
    {
        gameStates = GameObject.Find("GameManager").GetComponent<GameStates>();
    }

    void Update()
    {
        TimeContorl(KeyCode.Space);

        MainGameMenu(KeyCode.Escape);
    }

    void MainGameMenu(KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode)
            && MainMenu.activeInHierarchy == false
            && gameStates.BuildingGameState == false
            && gameStates.CreatingLandGameState == false)
        {
            lastTimeScale = Time.timeScale;
            MainMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && MainMenu.activeInHierarchy == true)
        {
            MainMenu.SetActive(false);
            Time.timeScale = lastTimeScale;
        }
    }

    void TimeContorl(KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode))
        {
            //Debug.Log("Time Scale: " + Time.timeScale);
            if (Time.timeScale == 1f)
            {
                TimeNote.SetActive(true);
                Time.timeScale = Time.timeScale - 1f;
            }

            else if (Time.timeScale == 0f)
            {
                TimeNote.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
}

