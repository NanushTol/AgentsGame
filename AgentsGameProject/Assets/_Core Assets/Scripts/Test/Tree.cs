using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tree : MonoBehaviour
{
    public float size;

    [HideInInspector]
    public Wood wood;

    private void Awake()
    {
        wood = gameObject.AddComponent<Wood>();
        wood.Amount = size;
    }
}
