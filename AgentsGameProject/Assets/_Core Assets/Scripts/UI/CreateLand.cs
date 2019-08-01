using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using static Constants;

public class CreateLand : MonoBehaviour
{
    public float LandCost = 3f;
    Grid mapGrid;
    public Tilemap LandTileMap;
    public Tilemap WaterTileMap;
    public TileBase LandTile;
    public TileBase WaterTile;
    public GameObject note;

    public MapCreator mapCreator;

    GameObject hoverTile;
    Vector3 hoverTileOffset = new Vector3(0.5f, 0.5f, 0f);

    ResourcesDataController resourcesDataController;
    LevelManager LevelManagerRef;

    float _godForce;

    [HideInInspector]
    public bool creatingLand = false;

    void Awake()
    {
        hoverTile = GameObject.Find("HoverTile");
        mapGrid = GameObject.Find("Grid").GetComponent<Grid>();

        resourcesDataController = GameObject.Find("GameManager").GetComponent<ResourcesDataController>();
        LevelManagerRef = GameObject.Find("GameManager").GetComponent<LevelManager>();
    }


    public void CreatingLand()
    {
        note.SetActive(true);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // get the collision point of the ray with the z = 0 plane
        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        Vector3Int position = mapGrid.WorldToCell(worldPoint);

        hoverTile.transform.position = position + hoverTileOffset;

        _godForce = resourcesDataController.GetResourceAmount(GODFORCE);

        if (Input.GetMouseButton(0))
        {
            if (_godForce > LandCost && LandTileMap.GetTile(position) == null) //create land on left click
            {
                LandTileMap.SetTile(position, LandTile);  //remove water
                WaterTileMap.SetTile(position, null); // create land

                resourcesDataController.UpdateResourceAmount(GODFORCE, -LandCost); // update god force
                resourcesDataController.UpdateResourcesValue();

                //refresh colliders
                WaterTileMap.GetComponent<CompositeCollider2D>().enabled = false;
                WaterTileMap.GetComponent<CompositeCollider2D>().enabled = true;

                // update positions for pathfinder
                position.x += mapCreator.MapWidth / 2;
                position.y += mapCreator.MapHeight / 2;
                Utils.UpdatePathfinderNode(position, true);
            }
        }

        if (Input.GetMouseButton(1)) // remove land on right click
        {
            if (LandTileMap.GetTile(position) != null)
            {
                LandTileMap.SetTile(position, null); //remove land
                WaterTileMap.SetTile(position, WaterTile); // return water

                //refresh colliders
                WaterTileMap.GetComponent<CompositeCollider2D>().enabled = false;
                WaterTileMap.GetComponent<CompositeCollider2D>().enabled = true;

                resourcesDataController.UpdateResourceAmount(GODFORCE, LandCost); ; // update god force
                resourcesDataController.UpdateResourcesValue();

                // update positions for pathfinder
                position.x += mapCreator.MapWidth / 2;
                position.y += mapCreator.MapHeight / 2;

                Utils.UpdatePathfinderNode(position, false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            note.SetActive(false);
            hoverTile.transform.position = new Vector3(0f, -20f, 0f);
            LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.BaseState]);
        }
    }

    public void CreateLandFunc()
    {
        LevelManagerRef.StateMachineRef.ChangeState(LevelManagerRef.States[LevelManager.StatesEnum.CreatingLand]);
    }
}
