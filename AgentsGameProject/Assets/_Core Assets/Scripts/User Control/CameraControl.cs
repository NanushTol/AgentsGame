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

    public MapCreator MapCreator;

    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;

    private float _mapX;
    private float _mapY;

    float _vertExtent;
    float _horzExtent;

    // Start is called before the first frame update
    void Start()
    {
        cameraSize = GetComponent<Camera>().orthographicSize;

        _mapX = MapCreator.MapWidth;
        _mapY = MapCreator.MapHeight;
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

        CalculatePositionLimits();
    }

    private void LateUpdate()
    {
        LimitCameraPositionToMap();
    }


    void LimitCameraPositionToMap()
    {
        Vector3 v3 = transform.position;
        v3.x = Mathf.Clamp(v3.x, _minX, _maxX);
        v3.y = Mathf.Clamp(v3.y, _minY, _maxY);
        transform.position = v3;
    }

    void CalculatePositionLimits()
    {
        _vertExtent = Camera.main.orthographicSize;
        _horzExtent = _vertExtent * Screen.width / Screen.height;

        _minX = _horzExtent - _mapX / 2;
        _maxX = _mapX / 2 - _horzExtent;
        _minY = _vertExtent - _mapY / 2;
        _maxY = _mapY / 2 - _vertExtent;
    }
}
