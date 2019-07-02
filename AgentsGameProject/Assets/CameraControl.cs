using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float ScrollSens = 1f;
    float cameraSize;

    Vector3 cameraPos;

    public float sensitivity = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        cameraSize = GetComponent<Camera>().orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        cameraSize -= Input.mouseScrollDelta.y * ScrollSens;
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(cameraSize, 5f, 18f);

        if (Input.GetMouseButton(2))
        {
            var mouseX = Input.GetAxis("MouseX");
            var mouseY = Input.GetAxis("MouseY");


            cameraPos = transform.position;
            cameraPos += transform.right * mouseX * sensitivity * (-1);
            cameraPos += transform.up * mouseY * sensitivity * (-1);
            transform.position = cameraPos;
        }
    }
}
