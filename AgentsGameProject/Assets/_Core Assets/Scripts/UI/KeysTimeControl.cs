using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysTimeControl : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown("[+]"))
        {
            //Debug.Log("scale +");
            Time.timeScale += 1;
            
        }

        if (Input.GetKeyDown("[-]"))
        {
            if(Time.timeScale > 1)
            {
                //Debug.Log("scale -");
                Time.timeScale -= 1;
            }

            if (Time.timeScale == 1)
            {
                Time.timeScale -= 1f;
            }

            if(Time.timeScale == 0)
            {
                Time.timeScale = 0f;
            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if(Time.timeScale > 0)
            {
                //Debug.Log("Paused");

                Time.timeScale = 0.0f;
            }
            if(Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }
        }
    }
}
