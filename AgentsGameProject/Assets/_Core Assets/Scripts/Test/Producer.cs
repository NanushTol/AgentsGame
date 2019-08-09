using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class Producer : MonoBehaviour
{
    public ProducerTypeSO ProducerType;

    public float[] MaterialsOutput;
    public float[] ElementsOutput;
    public float[] ForcesOutput;

    Medium _airMedium;
    Vector2Int _position;

    public void Initialize(Medium airMedium, ProducerTypeSO type)
    {
        _airMedium = airMedium;
        ProducerType = type;

        MaterialsOutput = ProducerType.Materials;
        ElementsOutput = ProducerType.Elements;
        ForcesOutput = ProducerType.Forces;
    }

    void Produce()
    {
        _position = new Vector2Int((int)Math.Round(this.transform.position.x), (int)Math.Round(this.transform.position.y));

        MediumCell cell = _airMedium.GetCellByPosition(_position);

        if (ProducerType.ProduceElements)
        {
            for (int i = 0; i < ElementsOutput.Length; i++)
            {
                cell.Content[i] += ElementsOutput[i] * Time.deltaTime;
            }
        }
    }

    private void Update()
    {
        Produce();
    }
}
