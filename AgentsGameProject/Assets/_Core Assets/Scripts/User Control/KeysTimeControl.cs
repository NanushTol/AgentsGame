using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysTimeControl : MonoBehaviour
{

    public GameObject TimeNote;



    void Update()
    {
        if (Input.GetKeyDown("[-]"))
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
/*
        if (Input.GetKeyDown("[+]"))
        {
            //Debug.Log("scale +");
            Time.timeScale += 1f;
            
        }

        if (Input.GetKeyDown("[-]"))
        {
            if(Time.timeScale >= 1f)
            {
                //Debug.Log("scale -");
                Time.timeScale = Time.timeScale - 1f;
            }

            if (Time.timeScale < 1f)
            {
                Time.timeScale = 0f;
            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if(Time.timeScale > 0f)
            {
                //Debug.Log("Paused");

                Time.timeScale = 0.0f;
            }
            if(Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }
        }*/
