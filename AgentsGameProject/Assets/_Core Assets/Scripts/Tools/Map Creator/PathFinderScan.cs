using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class PathFinderScan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        // Set Pathfinder Grid Graph
        var gg = AstarPath.active.data.gridGraph;
        gg.center = new Vector3(0, 0, 0);
        gg.SetDimensions(GetComponent<MapCreator>().MapWidth, GetComponent<MapCreator>().MapHeight, 1);

        AstarPath.active.Scan();

        //Debug.Log("Scaned");
    }

}
