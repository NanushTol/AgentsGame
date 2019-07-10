using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float ScrollSensitivity = 1f;
    float cameraSize;

    Vector3 cameraPos;

    public float PanSensitivity = 0.5f;

    public float MinZoom;
    public float MaxZoom;


    // Start is called before the first frame update
    void Start()
    {
        cameraSize = GetComponent<Camera>().orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        cameraSize -= Input.mouseScrollDelta.y * ScrollSensitivity;
        cameraSize = Mathf.Clamp(cameraSize, MinZoom, MaxZoom);
        GetComponent<Camera>().orthographicSize = cameraSize;

        if (Input.GetMouseButton(2))
        {
            var mouseX = Input.GetAxis("MouseX");
            var mouseY = Input.GetAxis("MouseY");

            // Camera Pan
            cameraPos = transform.position;
            cameraPos += transform.right * mouseX * PanSensitivity * (-1) * cameraSize;
            cameraPos += transform.up * mouseY * PanSensitivity * (-1) * cameraSize;
            transform.position = cameraPos;
        }
    }
}
