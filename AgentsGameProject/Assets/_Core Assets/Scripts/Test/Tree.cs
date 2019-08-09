using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tree : MonoBehaviour
{
    public float size;

    [HideInInspector]
    public Wood wood;
    [HideInInspector]
    public Producer producer;
    public ProducerTypeSO ProducerType;

    Medium _airMedium;

    private void Awake()
    {
        _airMedium = GameObject.Find("MediumAir").GetComponent<Medium>();

        wood = gameObject.AddComponent<Wood>();
        producer = gameObject.AddComponent<Producer>();
        producer.Initialize(_airMedium, ProducerType);

        wood.Amount = size;
    }
}
