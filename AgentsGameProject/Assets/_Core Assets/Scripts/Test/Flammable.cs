using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;


public class Flammable : MonoBehaviour
{
    public GameMaterial Owner;

    public bool _burning = false;

    public GameObject FirePrefab;

    private FlammableTypeSO _flammableType;

    Vector2Int _cellPosition;

    Medium _airMedium;

    [HideInInspector]
    public float[] IOElements;

    public void Initialize(GameMaterial owner, FlammableTypeSO flammableType)
    {
        _flammableType = flammableType;
        Owner = owner;
        IOElements = new float[5];
        _airMedium = GameObject.Find("MediumAir").GetComponent<Medium>();

        for (int i = 0; i < 5; i++)
        {
            IOElements[i] = _flammableType.IO[i].ValuePerTick;
        }
    }

    public void Burn()
    {
        _burning = true;
        GameObject fire = Instantiate(FirePrefab, transform.position, Quaternion.identity);
        fire.transform.parent = transform;
        fire.GetComponent<Fire>().Owner = this.Owner;
    }

    private void Update()
    {
        if (_burning)
        {
            Owner.Amount -= 0.5f * Time.deltaTime;

            _cellPosition = new Vector2Int((int)Math.Round(this.transform.position.x), (int)Math.Round(this.transform.position.y));

            MediumCell cell = _airMedium.GetCellByPosition(_cellPosition);

            for (int i = 0; i < IOElements.Length; i++)
            {
                cell.Content[i] += IOElements[i] * Time.deltaTime;
            }
        }
    }
} 

