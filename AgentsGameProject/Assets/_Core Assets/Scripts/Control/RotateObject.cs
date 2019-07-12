using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class RotateObject : MonoBehaviour

{
    GameObject globalStats;

    public  float _sensitivity = 0.4f;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;

    void Awake()
    {
        _sensitivity = 0.4f;
        _rotation = Vector3.zero;
        globalStats = GameObject.Find("GlobalStats");
    }

    void OnMouseDrag()
    {
        if (globalStats.GetComponent<GlobalStats>().rotating)
        {
            // offset
            _mouseOffset = (Input.mousePosition - _mouseReference);

            // apply rotation
            _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;

            // rotate
            transform.Rotate(_rotation);

            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        if(globalStats.GetComponent<GlobalStats>().rotating)
        {
            // rotating flag
            //_isRotating = true;

            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }

    //void OnMouseUp()
    //{
        // rotating flag
    //    _isRotating = false;
    //}

}