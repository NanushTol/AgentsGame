using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;
using WeatherSystem;


/// <summary>
/// Flamable is attached to any flamabale material ie: Wood, Flesh, Oil, Gas
/// 
/// It handles the Elements I\O to and from the different Mediums
/// 
/// Flamable IOElements Order:
/// Water = 1
/// Co2 = 2
/// Oxygen = 3
/// Nutrients = 4
/// Heat = 5
/// </summary>

public class Flammable : MonoBehaviour
{

    #region Variables
    public GameMaterial Owner;

    public bool _burning = false;

    public GameObject FirePrefab;

    private FlammableTypeSO _flammableType;

    Vector2Int _cellPosition;

    Medium _airMedium;

    GameObject _fire;

    [HideInInspector]
    public float[] IOElements;
    #endregion

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
        if (!Owner.Wet)
        {
            _burning = true;
            _fire = Instantiate(FirePrefab, transform.position, Quaternion.identity);
            _fire.transform.parent = transform;
            _fire.GetComponent<Fire>().Owner = this.Owner;
        }
    }

    public void Burning()
    {
        Owner.Amount -= 0.5f * Time.deltaTime;

        _cellPosition = new Vector2Int((int)Math.Round(this.transform.position.x), (int)Math.Round(this.transform.position.y));

        MediumCell cell = _airMedium.GetCellByPosition(_cellPosition);

        for (int i = 0; i < IOElements.Length; i++)
        {
            if (i != 2)
                cell.Content[i] += IOElements[i] * Time.deltaTime;
            else // Oxygen
            {
                if (cell.Content[i] > 0)
                {
                    cell.Content[i] -= IOElements[i] * Time.deltaTime;
                }
                else //if no Oxygen Fire dies
                {
                    _burning = false;
                    Destroy(_fire.gameObject);
                }
            }
        }
    }

    private void Update()
    {
        if (_burning && !Owner.Wet)
            Burning();
        else if (_burning && Owner.Wet)
            _burning = false;
    }
} 

