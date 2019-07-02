using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float ScrollSens = 1f;
    float cameraSize;

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
    }
}
