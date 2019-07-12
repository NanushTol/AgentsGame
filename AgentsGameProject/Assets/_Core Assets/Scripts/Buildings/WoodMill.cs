﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodMill : MonoBehaviour
{
    public float WorkersRadius = 1.5f;

    public float WoodMillEfficiency = 2.3f;

    public int MaxWorkers = 6;

    public Color WorkingColor = new Color(0.52f, 0.96f, 0.27f, 1f);
    public Color NotWorkingColor = new Color(0.65f, 0.65f, 0.65f, 1f);


    [HideInInspector]
    public int CurrntlyWorking;

    float spherecastTimer = 0;

    int[] agentsWorking = new int[0];

    bool PositionValid;

    GenericBuilding genericBuilding;
    Environment environment;
    ResourcesData resourcesData;

    void Awake()
    {
        environment = GameObject.Find("Environment").GetComponent<Environment>();
        resourcesData = GameObject.Find("GameManager").GetComponent<ResourcesData>();

        genericBuilding = GetComponent<GenericBuilding>();

        genericBuilding.WorkEfficiency = WoodMillEfficiency;
    }


    void Update()
    {
        spherecastTimer = spherecastTimer + Time.deltaTime;

        //check for working workers
        if (spherecastTimer >= 0.5)
        {
            Collider2D[] _objectColliders = Physics2D.OverlapCircleAll(transform.position, WorkersRadius, LayerMask.GetMask("Agent"));



            CurrntlyWorking = 0;


            // count how many workers
            for (int a = 0; a < _objectColliders.Length; a++)
            {
                if (_objectColliders[a].GetComponent<Agent>().mostUrgentNeedIndex == 2) // most urgent need = Work
                {
                    CurrntlyWorking += 1;
                }
            }

            agentsWorking = new int[CurrntlyWorking];


            //brodcast "need workers"
            if (agentsWorking.Length <= 6)
            {

                genericBuilding.WorkersNeeded = true;

                if (agentsWorking.Length == 6)
                {
                    genericBuilding.WorkersNeeded = false;
                }

            }

            if (agentsWorking.Length > 6)
            {
                agentsWorking = new int[6];
                genericBuilding.WorkersNeeded = false;
            }


            spherecastTimer = 0;
        }

        UpdateVacancyBar(agentsWorking);

        if (genericBuilding.Production > 0)
        {
            resourcesData.WoodProduction = genericBuilding.WorkEfficiency * Time.deltaTime * agentsWorking.Length;
            resourcesData.WoodAmount += resourcesData.WoodProduction;
            resourcesData.WoodProduction *= (1f / Time.deltaTime); // display production per second and not per frame
            genericBuilding.Production = genericBuilding.Production - (genericBuilding.WorkEfficiency * Time.deltaTime * agentsWorking.Length);

            //environment.WorkTempIn += environment.WorkTempCost * agentsWorking.Length;
        }


    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, WorkersRadius);
    }


    void UpdateVacancyBar(int[] _agentsWorking)
    {
        Transform vacancyBar = transform.GetChild(0);

        for (int j = 0; j < MaxWorkers; j++)
        {
            vacancyBar.transform.GetChild(j).gameObject.GetComponent<SpriteRenderer>().color = NotWorkingColor;
        }

        for (int i = 0; i < _agentsWorking.Length; i++)
        {
            vacancyBar.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = WorkingColor;
        }

    }
}