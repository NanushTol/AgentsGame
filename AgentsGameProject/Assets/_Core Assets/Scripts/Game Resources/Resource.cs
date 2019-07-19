
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class Resource : MonoBehaviour
{
    public Grid grid;

    public enum TypeOfResource {Stone, Wood, Mineral, LandOil, WaterOil, Thermal, Wind, Solar }
    public TypeOfResource typeOfResource;

    [HideInInspector]
    public Vector3Int LastPosition;

    public float Amount;
    TextMeshProUGUI amountUiElement;

    public Color WoodResourceColor;
    public Color StoneResourceColor;
    public Color MineralResourceColor;
    public Color EnergyResourceColor;


    // Start is called before the first frame update
    void Awake()
    {
        //environment = GameObject.Find("Environment").GetComponent<Environment>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();

        amountUiElement = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        amountUiElement.text = Amount.ToString();

        switch (typeOfResource)
        {
            case TypeOfResource.Wood:
                transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = WoodResourceColor;
                break;
            case TypeOfResource.Stone:
                transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = StoneResourceColor;
                break;
            case TypeOfResource.Mineral:
                transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = MineralResourceColor;
                break;
            case TypeOfResource.LandOil:
                transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = EnergyResourceColor;
                break;
        }
    }
    private void Start()
    {
        Vector3Int position = grid.WorldToCell(transform.position);

        position.x += 18;
        position.y += 17;

        LastPosition = position;

        var gg = AstarPath.active.data.gridGraph;
        int x = position.x;
        int y = position.y;
        GridNodeBase node = gg.GetNode(x, y);

        AstarPath.active.AddWorkItem(ctx => {
            var PfGridGraph = AstarPath.active.data.gridGraph;

            // Mark a single node as unwalkable
            PfGridGraph.GetNode(x, y).Walkable = false;

            // Recalculate the connections for that node as well as its neighbours
            PfGridGraph.CalculateConnectionsForCellAndNeighbours(x, y);
        });
    }

    void Update()
    {
        amountUiElement.text = Amount.ToString();
    }

    public void UpdateNode(Vector3Int _position, bool _walkable)
    {
        AstarPath.active.AddWorkItem(ctx => {
            var PfGridGraph = AstarPath.active.data.gridGraph;

            // Mark a single node as unwalkable
            PfGridGraph.GetNode(_position.x, _position.y).Walkable = _walkable;

            // Recalculate the connections for that node as well as its neighbours
            PfGridGraph.CalculateConnectionsForCellAndNeighbours(_position.x, _position.y);
        });
    }


}
