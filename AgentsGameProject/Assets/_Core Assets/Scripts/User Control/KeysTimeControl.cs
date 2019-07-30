using UnityEngine;

public class KeysTimeControl : MonoBehaviour
{

    public GameObject TimeNote;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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

