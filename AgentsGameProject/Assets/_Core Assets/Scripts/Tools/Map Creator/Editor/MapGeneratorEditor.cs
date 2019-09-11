using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using WeatherSystem;


[CustomEditor(typeof(MapCreator))]
public class MapGeneratorEditor : Editor
{
    GameObject LandTileMap;
    public override void OnInspectorGUI()
    {
        MapCreator mapCreator = (MapCreator)target;

        mapCreator.MapWidth = EditorGUILayout.IntField("Map Width", mapCreator.MapWidth);
        mapCreator.MapHeight = EditorGUILayout.IntField("Map Height", mapCreator.MapHeight);

        EditorGUILayout.Space();

        mapCreator.LandWidth = EditorGUILayout.IntField("Land Width", mapCreator.LandWidth);
        mapCreator.LandHeight = EditorGUILayout.IntField("Land Height", mapCreator.LandHeight);


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();



        mapCreator.LandTileMap = (Tilemap)EditorGUILayout.ObjectField("Land Tilemap", mapCreator.LandTileMap, typeof(Tilemap) , true);
        mapCreator.WaterTileMap = (Tilemap)EditorGUILayout.ObjectField("Water Tilemap", mapCreator.WaterTileMap, typeof(Tilemap), true);


        EditorGUILayout.Space();

        mapCreator.LandTile = (TileBase)EditorGUILayout.ObjectField("Land Tile", mapCreator.LandTile, typeof(TileBase), true);
        mapCreator.WaterTile = (TileBase)EditorGUILayout.ObjectField("Water Tile", mapCreator.WaterTile, typeof(TileBase), true);
        mapCreator.MediumTile = (TileBase)EditorGUILayout.ObjectField("Medium Tile", mapCreator.MediumTile, typeof(TileBase), true);


        EditorGUILayout.Space();
        EditorGUILayout.Space();

        mapCreator.AirMedium = (Medium)EditorGUILayout.ObjectField("Air Medium", mapCreator.AirMedium, typeof(Medium), true);


        if (GUILayout.Button("Create Map"))
        {
            mapCreator.CreateMap();
        }


        EditorGUILayout.Space();


    }

}
