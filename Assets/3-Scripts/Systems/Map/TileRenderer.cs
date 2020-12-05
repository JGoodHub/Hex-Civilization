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
        public Vector2Int minCoords;
        public GameObject container;
        public RuntimeCombiner combiner;
    }

    [Header("Generation Settings")]
    public bool randomRotation;
    public int batchSize = 5;
    public Transform tileContainer;
    private List<TileChunk> tileChunks = new List<TileChunk>();

    [Header("Nature Tiles Prefabs")]
    public GameObject oceanTile;
    public GameObject beachTile;
    public GameObject desertTile;
    public GameObject grassTile;
    public GameObject flowersTile;
    public GameObject forestTile;

    [Header("Tile Palette")]
    public Material tilePaletteMaterial;

    private void Start()
    {
        CreateChunks();

        DrawAllChunks();

        GC.Collect();
        Resources.UnloadUnusedAssets();

        Application.targetFrameRate = -1;
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

        return Instantiate(tilePrefab, TileGenerator.GridToWorldPosition(x, y), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), tileContainer);
    }

    public void CreateChunks()
    {
        Vector2Int mapSize = TileGenerator.Instance.mapSize;

        //Group tiles by chunk
        for (int y = 0; y <= mapSize.y - batchSize; y += batchSize)
        {
            for (int x = 0; x <= mapSize.x - batchSize; x += batchSize)
            {
                TileChunk newChunk = new TileChunk();

                newChunk.minCoords = new Vector2Int(x, y);
                newChunk.container = new GameObject($"Chunk_({x}, {y})");
                newChunk.container.transform.parent = tileContainer;

                tileChunks.Add(newChunk);
            }
        }

        //Convert chunks to single mesh objects
        for (int i = 0; i < tileChunks.Count; i++)
        {
            tileChunks[i].combiner = tileChunks[i].container.AddComponent<RuntimeCombiner>();
            tileChunks[i].combiner.sharedMaterial = tilePaletteMaterial;

            tileChunks[i].combiner.combineOnStart = false;
            tileChunks[i].combiner.useInt32Buffers = true;
            tileChunks[i].combiner.destroyChildMeshes = true;
            tileChunks[i].combiner.destroyAllChildren = true;

        }
    }

    public TileChunk GetChunkContainingTile(int x, int y)
    {
        if (x < 0 || x >= TileGenerator.Instance.mapSize.x || y < 0 || y >= TileGenerator.Instance.mapSize.y)
            return null;

        foreach (TileChunk chunk in tileChunks)
            if (x >= chunk.minCoords.x && x < chunk.minCoords.x + batchSize && y >= chunk.minCoords.y && y < chunk.minCoords.y + batchSize)
                return chunk;

        return null;
    }

    public void DrawChunk(TileChunk chunk)
    {
        //Get a local copy of the tile map data for just this chunk
        TileData.Type[,] chunkMap = new TileData.Type[batchSize, batchSize];

        for (int y = 0; y < batchSize; y++)
            for (int x = 0; x < batchSize; x++)
                chunkMap[x, y] = TileGenerator.Instance.map[x + chunk.minCoords.x, y + chunk.minCoords.y];

        //Place new tiles based on the map data
        for (int y = 0; y < batchSize; y++)
            for (int x = 0; x < batchSize; x++)
                PlaceTile(chunkMap[x, y], x + chunk.minCoords.x, y + chunk.minCoords.y).transform.parent = chunk.container.transform;

        chunk.combiner.Combine();
    }

    public void DrawAllChunks()
    {
        foreach (TileChunk chunk in tileChunks)
            DrawChunk(chunk);

        int vertexCount = 0;
        foreach (TileChunk chunk in tileChunks)
            vertexCount += chunk.container.GetComponent<MeshFilter>().sharedMesh.vertexCount;

        Debug.Log(vertexCount);
    }

}