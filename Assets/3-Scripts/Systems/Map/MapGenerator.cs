using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Generation Resources")]
    public Transform tileContainer;
    public GameObject[] tilePrefabs;


    [Header("Generation Parameters")]
    public int seed;
    public Vector2Int boardSize;

    [Header("Continental Settings")]
    [Range(1, 6)] public int numberOfContinents;
    [Range(0, 100)] public int maxLandPercentage;

    [Header("Altitude Settings")]
    public float altitudeNoiseScale = 5f;
    [Range(0f, 1f)] public float oceansBoundary = 0.25f;
    [Range(0f, 1f)] public float moutainsBoundary = 0.75f;

    [Header("Latitude Settings")]
    public float latitudeNoiseScale = 5f;
    [Range(0f, 1f)] public float articBoundary = 0.75f;
    [Range(0f, 1f)] public float tundraBoundary = 0.75f;
    [Range(0f, 1f)] public float grasslandBoundary = 0.75f;
    [Range(0f, 1f)] public float desertBoundary = 0.25f;

    private HashSet<GameObject> runtimeTiles = new HashSet<GameObject>();

    private void Start()
    {
        Random.InitState(seed);

        for (int y = 0; y < boardSize.y; y++)
        {
            for (int x = 0; x < boardSize.x; x++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                float sample = GetAltitudeNoiseSample(gridPosition);


                int tileIndex = Mathf.FloorToInt(sample * tilePrefabs.Length);

                GameObject tileClone = Instantiate(tilePrefabs[tileIndex], GridToWorldPosition(gridPosition), Quaternion.identity);

                tileClone.transform.parent = tileContainer;
                runtimeTiles.Add(tileClone);
            }
        }

        Camera.main.transform.position = (Vector3)GridToWorldPosition(new Vector2Int(boardSize.x / 2, boardSize.y / 2)) + (Vector3.back * 10);
    }

    public static Vector2 GridToWorldPosition(Vector2Int position)
    {
        return new Vector2((position.x * TileData.SIZE.x) + (position.y % 2 * (TileData.SIZE.x / 2)), position.y * (TileData.SIZE.y * 0.75f));
    }

    public static Vector2Int WorldToGridPosition(Vector2 position)
    {
        return Vector2Int.zero;
    }

    private float GetAltitudeNoiseSample(Vector2Int gridPos)
    {
        Vector2 samplePosition = GridToWorldPosition(gridPos) * altitudeNoiseScale;
        return Mathf.Clamp(Mathf.PerlinNoise(samplePosition.x, samplePosition.y), 0f, 0.9999f);
    }

    [Header("Debug")]
    public bool drawLatitudeBoundaries;
    private void OnDrawGizmos()
    {
        if (drawLatitudeBoundaries)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                float latPercentage = y < (boardSize.y - 1) / 2 ? y : (boardSize.y - 1) - y;
                latPercentage /= (boardSize.y - 1) / 2;

                Debug.Log(latPercentage);

                for (int x = 0; x < boardSize.x; x++)
                {
                    if (latPercentage <= articBoundary)
                        Gizmos.color = Color.white;

                    if (latPercentage > articBoundary && latPercentage <= tundraBoundary)
                        Gizmos.color = Color.grey;

                    if (latPercentage > tundraBoundary && latPercentage <= grasslandBoundary)
                        Gizmos.color = Color.green;

                    if (latPercentage > grasslandBoundary)
                        Gizmos.color = Color.yellow;

                    Gizmos.DrawCube(GridToWorldPosition(new Vector2Int(x, y)), TileData.SIZE);
                }
            }
        }
    }

}