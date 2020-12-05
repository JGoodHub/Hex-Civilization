using MellowMadness.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGenerator : Singleton<TileGenerator>
{

    [Header("Map Settings")]
    public bool useRandomSeed;
    public int seed;
    public Vector2Int mapSize = new Vector2Int(160, 90);
    private int mapArea;

    [Header("Ocean Settings")]
    public int verticalFallOff = 15;
    public int horizontalFallOff = 10;

    [Header("Land Settings")]
    [Range(0, 50)] public int landSeedPoints = 20;
    [Range(0, 100)] public int landIterations = 20;
    private int landThreshold = 3;

    [Header("Island Settings")]
    [Range(0, 10)] public int islandSeedPoints = 5;
    [Range(0, 6)] public int islandMinExpansions = 0;
    [Range(0, 6)] public int islandMaxExpansions = 3;
    [Range(0, 10)] public int islandMinDistance = 3;

    [Header("Beach Settings")]
    [Range(0, 4)] public int adjacentOceanThreshold = 1;
    public bool cullStrayBeaches;

    [Header("Forest Settings")]
    [Range(0f, 1f)] public float forestFill;
    [Range(0f, 1f)] public float forestNoiseScale;
    public bool removeBeachForests;

    public TileData.Type[,] map;

    void Start()
    {
        if (useRandomSeed)
            seed = int.Parse("" + System.DateTime.Now.Minute + System.DateTime.Now.Second + System.DateTime.Now.Millisecond);

        Random.InitState(seed);

        int batchSize = TileRenderer.Instance.batchSize;
        mapSize.x += mapSize.x % batchSize == 0 ? 0 : batchSize - (mapSize.x % batchSize);
        mapSize.y += mapSize.y % batchSize == 0 ? 0 : batchSize - (mapSize.y % batchSize);
        mapArea = mapSize.x * mapSize.y;

        ClearMap();

        GenerateContinents();

        GenerateIslands();

        CleanBorders();

        GenerateBeaches();

        GenerateForests();

        //AdjustSeaLevel();
    }

    private void ClearMap()
    {
        map = new TileData.Type[mapSize.x, mapSize.y];
    }

    private void GenerateContinents()
    {
        for (int s = 0; s < landSeedPoints; s++)
            map[Random.Range(horizontalFallOff, mapSize.x - horizontalFallOff - 1), Random.Range(verticalFallOff, mapSize.y - verticalFallOff - 1)] = TileData.Type.GRASS;

        for (int i = 0; i < landIterations; i++)
        {
            TileData.Type[,] newMap = (TileData.Type[,])map.Clone();

            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    int landTileCount = AdjacentTileCount(x, y, TileData.Type.GRASS);
                    int oceanTileCount = AdjacentTileCount(x, y, TileData.Type.OCEAN);

                    if (map[x, y] == TileData.Type.GRASS)
                    {
                        if (oceanTileCount > 0)
                        {
                            Vector2Int[] surroundingTileCoords = AdjacentTiles(x, y);
                            Vector2Int randomTile = surroundingTileCoords[Random.Range(0, surroundingTileCoords.Length)];
                            newMap[randomTile.x, randomTile.y] = TileData.Type.GRASS;
                        }
                    }
                    else
                    {
                        if (landTileCount >= landThreshold)
                        {
                            newMap[x, y] = TileData.Type.GRASS;
                        }
                    }
                }
            }

            map = newMap;
        }


    }

    private void GenerateIslands()
    {
        Vector2Int[] islandSeeds = new Vector2Int[islandSeedPoints];

        int islandIndex = 0;
        int loopEscape = 0;
        while (islandIndex < islandSeedPoints && loopEscape < 1000)
        {
            Vector2Int point = new Vector2Int(Random.Range(horizontalFallOff, mapSize.x - horizontalFallOff - 1), Random.Range(verticalFallOff, mapSize.y - verticalFallOff - 1));

            if (map[point.x, point.y] == TileData.Type.OCEAN && DistanceToNearestTile(point, TileData.Type.GRASS) >= islandMinDistance)
            {
                map[point.x, point.y] = TileData.Type.GRASS;
                islandSeeds[islandIndex] = point;
                islandIndex++;
            }

            loopEscape++;
        }

        foreach (Vector2Int islandSeed in islandSeeds)
        {
            List<Vector2Int> islandTiles = new List<Vector2Int>();
            islandTiles.Add(islandSeed);

            Vector2Int[] expansionTiles = AdjacentTiles(islandSeed);
            ShuffleTileArray(ref expansionTiles);

            int expansions = Random.Range(islandMinExpansions, islandMaxExpansions + 1);
            for (int e = 0; e < expansions; e++)
            {
                map[expansionTiles[e].x, expansionTiles[e].y] = TileData.Type.GRASS;
                islandTiles.Add(expansionTiles[e]);
            }

            for (int i = 0; i < islandTiles.Count; i++)
            {
                Vector2Int[] shellTiles = AdjacentTiles(islandTiles[i]);

                for (int a = 0; a < shellTiles.Length; a++)
                {
                    map[shellTiles[a].x, shellTiles[a].y] = TileData.Type.GRASS;
                }
            }
        }
    }

    private void CleanBorders()
    {
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                if (x == 0 || y == 0 || x == mapSize.x - 1 || y == mapSize.y - 1)
                {
                    map[x, y] = TileData.Type.OCEAN;
                }
            }
        }
    }

    private void GenerateBeaches()
    {
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                if (map[x, y] == TileData.Type.GRASS)
                {
                    int oceanTileCount = AdjacentTileCount(x, y, TileData.Type.OCEAN);

                    if (cullStrayBeaches && oceanTileCount >= 4)
                    {
                        map[x, y] = TileData.Type.OCEAN;
                    }
                    else if (AdjacentTileCount(x, y, TileData.Type.OCEAN) >= adjacentOceanThreshold)
                    {
                        map[x, y] = TileData.Type.BEACH;
                    }
                }
            }
        }
    }

    private void GenerateForests()
    {
        for (int y = 0; y < mapSize.y; y++)
            for (int x = 0; x < mapSize.x; x++)
                if (map[x, y] == TileData.Type.GRASS && Mathf.PerlinNoise(x * forestNoiseScale, y * forestNoiseScale) <= forestFill)
                    map[x, y] = TileData.Type.FOREST;

        if (removeBeachForests)
            for (int y = 0; y < mapSize.y; y++)
                for (int x = 0; x < mapSize.x; x++)
                    if (map[x, y] == TileData.Type.FOREST && AdjacentTileCount(x, y, TileData.Type.BEACH) >= 1)
                        map[x, y] = TileData.Type.GRASS;

    }

    public static Vector3 GridToWorldPosition(int x, int y)
    {
        return GridToWorldPosition(new Vector2Int(x, y));
    }

    public static Vector3 GridToWorldPosition(Vector2Int gridCoord)
    {
        return new Vector3((gridCoord.x * TileData.SIZE.x) + (gridCoord.y % 2 * (TileData.SIZE.x / 2)), 0f, gridCoord.y * (TileData.SIZE.y * 0.75f));
    }

    public static Vector2Int WorldToGridPosition(Vector3 position)
    {
        return Vector2Int.zero;
    }

    public Vector2Int[] AdjacentTiles(int x, int y)
    {
        return AdjacentTiles(new Vector2Int(x, y));
    }

    public Vector2Int[] AdjacentTiles(Vector2Int gridCoord)
    {
        List<Vector2Int> adjacentCoords = new List<Vector2Int>();

        for (int yOff = -1; yOff <= 1; yOff++)
        {
            for (int xOff = -1; xOff <= 1; xOff++)
            {
                if (yOff == 0 && xOff == 0)
                    continue;
                else if (gridCoord.x + xOff < 0 || gridCoord.x + xOff >= mapSize.x)
                    continue;
                else if (gridCoord.y + yOff < 0 || gridCoord.y + yOff >= mapSize.y)
                    continue;
                else if (gridCoord.y % 2 == 0 && yOff != 0 && xOff == 1)
                    continue;
                else if (gridCoord.y % 2 == 1 && yOff != 0 && xOff == -1)
                    continue;
                else
                    adjacentCoords.Add(new Vector2Int(gridCoord.x + xOff, gridCoord.y + yOff));
            }
        }

        return adjacentCoords.ToArray();
    }

    public int AdjacentTileCount(int x, int y, TileData.Type type)
    {
        return AdjacentTileCount(new Vector2Int(x, y), type);
    }

    public int AdjacentTileCount(Vector2Int gridCoord, TileData.Type type)
    {
        int typeCount = 0;
        foreach (Vector2Int adjCoord in AdjacentTiles(gridCoord))
            if (map[adjCoord.x, adjCoord.y] == type)
                typeCount++;

        return typeCount;
    }

    public int DistanceToNearestTile(Vector2Int gridCoord, TileData.Type type)
    {
        HashSet<Vector2Int> openTiles = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closedTiles = new HashSet<Vector2Int>();

        HashSet<Vector2Int> tilesToAdd = new HashSet<Vector2Int>();
        HashSet<Vector2Int> tilesToRemove = new HashSet<Vector2Int>();

        openTiles.Add(gridCoord);
        int shellIndex = 0;

        while (openTiles.Count > 0)
        {
            shellIndex++;

            tilesToAdd.Clear();
            tilesToRemove.Clear();

            foreach (Vector2Int openTile in openTiles)
            {
                Vector2Int[] adjacentTiles = AdjacentTiles(openTile);
                foreach (Vector2Int adjacentTile in adjacentTiles)
                {
                    if (map[adjacentTile.x, adjacentTile.y] == type)
                    {
                        return shellIndex;
                    }
                    else if (openTiles.Contains(adjacentTile) == false && closedTiles.Contains(adjacentTile) == false)
                    {
                        tilesToAdd.Add(adjacentTile);
                    }
                }

                tilesToRemove.Add(openTile);
                closedTiles.Add(openTile);
            }

            openTiles.ExceptWith(tilesToRemove);
            openTiles.UnionWith(tilesToAdd);
        }

        return -1;
    }

    public void ShuffleTileArray(ref Vector2Int[] arr)
    {
        for (int s = 0; s < arr.Length; s++)
        {
            int indexA = Random.Range(0, arr.Length);
            int indexB = Random.Range(0, arr.Length);

            Vector2Int temp = arr[indexA];
            arr[indexA] = arr[indexB];
            arr[indexB] = temp;
        }
    }

    public void Regenerate()
    {
        Start();
        TileRenderer.Instance.DrawAllChunks();
    }

}
