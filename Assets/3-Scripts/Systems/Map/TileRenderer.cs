using MellowMadness.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileRenderer : Singleton<TileRenderer>
{

    public class TileChunk
    {
        public Vector2Int centerCoordinate;
        public GameObject[] tiles;
    }

    [Header("Generation Resources")]
    public Transform tileContainer;
    private Dictionary<TileData.Type, Transform> tileParents;
    private Dictionary<TileData.Type, List<TileChunk>> tileChuncks;
    private List<GameObject> runtimeTiles = new List<GameObject>();

    [Header("Nature Tiles")]
    public GameObject oceanTile;
    public GameObject beachTile;
    public GameObject desertTile;
    public GameObject grassTile;
    public GameObject flowersTile;
    public GameObject forestTile;

    private void Start()
    {
        RenderMap();

        GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    public void RenderMap()
    {
        SetupTileContainers();

        TileData.Type[,] tileMap = TileGenerator.Instance.map;

        for (int y = 0; y < tileMap.GetLength(1); y++)
        {
            for (int x = 0; x < tileMap.GetLength(0); x++)
            {
                runtimeTiles.Add(PlaceTile(tileMap[x, y], x, y));
            }
        }

        StaticBatchTiles();
    }

    public void EraseMap()
    {
        foreach (Transform parent in tileParents.Values)
        {
            Destroy(parent.gameObject);
        }

        runtimeTiles.Clear();
        tileParents.Clear();
    }

    public void UpdateChunk(int x, int y)
    {

    }

    private void SetupTileContainers()
    {
        tileParents = new Dictionary<TileData.Type, Transform>();
        //tileChuncks = new Dictionary<TileData.Type, List<TileChunk>>();

        foreach (TileData.Type tileType in Enum.GetValues(typeof(TileData.Type)))
        {
            Transform containerTransform = new GameObject(tileType + "_Container").transform;
            containerTransform.parent = tileContainer;
            tileParents.Add(tileType, containerTransform);
            //tileChuncks.Add(tileType, new List<TileChunk>());
        }
    }

    private GameObject PlaceTile(TileData.Type type, int x, int y)
    {
        GameObject tilePrefab;

        switch (type)
        {
            case TileData.Type.OCEAN:
                tilePrefab = oceanTile;
                break;
            case TileData.Type.BEACH:
                tilePrefab = beachTile;
                break;
            case TileData.Type.DESERT:
                tilePrefab = desertTile;
                break;
            case TileData.Type.GRASS:
                tilePrefab = grassTile;
                break;
            case TileData.Type.FLOWERS:
                tilePrefab = flowersTile;
                break;
            case TileData.Type.FOREST:
                tilePrefab = forestTile;
                break;
            default:
                tilePrefab = null;
                Debug.LogError($"Tile Generation Error: An incorrect tile type was passed for position {x}, {y}");
                break;
        }

        return Instantiate(tilePrefab, TileGenerator.GridToWorldPosition(x, y), Quaternion.identity, tileParents[type]);
    }

    private void StaticBatchTiles()
    {
        foreach (Transform parent in tileParents.Values)
        {
            RuntimeCombiner rtc = parent.gameObject.AddComponent<RuntimeCombiner>();
            rtc.UseFirstMaterial();
            rtc.useInt32Buffers = true;
        }

        //foreach (TileData.Type type in tileChuncks.Keys)
        //{
        //    GameObject[] combineBuffer = new GameObject[6];
        //    for (int i = 0; i < tileChuncks[type].Count; i++)
        //    {
        //        if (i == 6)
        //        {
        //            GameObject combineObj = new GameObject("Tiles_Batch");
        //            combineObj.transform.parent = tileParents[type];

        //            for (int b = 0; b < combineBuffer.Length; b++)
        //            {
        //                combineBuffer[b].transform.parent = combineObj.transform;
        //            }

        //            RuntimeCombiner rtCombine = combineObj.AddComponent<RuntimeCombiner>();
        //            rtCombine.UseFirstMaterial();
        //            rtCombine.useInt32Buffers = true;

        //            combineBuffer = new GameObject[combineBuffer.Length];
        //        }
        //        else
        //        {
        //            //combineBuffer[i % combineBuffer.Length] = tileChuncks[type][i];
        //        }

        //        GameObject combineObj1 = new GameObject("Tile_Batch");
        //        combineObj1.transform.parent = tileParents[type];

        //        for (int b = 0; b < combineBuffer.Length; b++)
        //        {
        //            combineBuffer[b].transform.parent = combineObj1.transform;
        //        }

        //        RuntimeCombiner rtCombine1 = combineObj1.AddComponent<RuntimeCombiner>();
        //        rtCombine1.UseFirstMaterial();
        //        rtCombine1.useInt32Buffers = true;

        //        combineBuffer = new GameObject[combineBuffer.Length];
        //    }
        //}
    }

}