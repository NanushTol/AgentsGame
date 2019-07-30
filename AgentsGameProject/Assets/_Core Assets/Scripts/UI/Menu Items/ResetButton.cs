using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public void resetButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("level", LoadSceneMode.Single);
    }
}
