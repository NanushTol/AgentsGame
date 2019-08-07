using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public float SpawnTime;
    float _elapsedTime;

    public GameObject CoinPrefab;

    public List<GameObject> SpawnerPoints;
    List<GameObject> EmptySpawner;


    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if(_elapsedTime >= SpawnTime)
        {
            EmptySpawner.Clear();

            foreach (GameObject spawner in SpawnerPoints)
            {
                if(!Physics2D.Raycast(spawner.transform.position, Camera.main.transform.forward, 50f, LayerMask.GetMask("CoinsLayer")))
                {
                    EmptySpawner.Add(spawner);
                }
            }

            int spawnerIndex = UnityEngine.Random.Range(0, EmptySpawner.Count -1);

            Instantiate(CoinPrefab, EmptySpawner[spawnerIndex].transform.position, Quaternion.identity);
        }
    }
}
