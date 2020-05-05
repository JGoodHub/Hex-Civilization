using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    [System.Serializable]
    public struct Biome {
        public string name;
        public GameObject[] primarys;
        public GameObject[] boundarys;
    }

    [Header("Generation Resources")]
    public Transform tileContainer;
    public Biome[] biomes;

    [Header("Generation Parameters")]
    public int seed;
    public Vector2Int boardSize;

    [Header("Elevation Settings")]
    public float elevationNoiseScale = 5f;
    [Range(0f, 1f)] public float oceansBoundary = 0.25f;
    [Range(0f, 1f)] public float moutainsBoundary = 0.75f;

    private HashSet<GameObject> tileObjects = new HashSet<GameObject>();

    private void Start() {
        Random.InitState(seed);

        for (int y = 0; y < boardSize.y; y++) {
            for (int x = 0; x < boardSize.x; x++) {
                Vector2Int gridPosition = new Vector2Int(x, y);
                int elevationIndex = GetElevationAtGridPosition(gridPosition);

                GameObject tilePrefab = biomes[elevationIndex].primarys[Random.Range(0, biomes[elevationIndex].primarys.Length)];
                GameObject tileClone = Instantiate(tilePrefab, GridToWorldPosition(gridPosition), Quaternion.identity);

                tileObjects.Add(tileClone);
                tileClone.transform.parent = tileContainer;
            }
        }

        Camera.main.transform.position = (Vector3)GridToWorldPosition(new Vector2Int(boardSize.x / 2, boardSize.y / 2)) + (Vector3.back * 10);
    }

    private Vector2 GridToWorldPosition(Vector2Int position) {
        return new Vector2((position.x * Tile.SIZE.x) + (position.y % 2 * (Tile.SIZE.x / 2)), position.y * (Tile.SIZE.y * 0.75f));
    }

    private Vector2Int WorldToGridPosition(Vector2 position) {
        return Vector2Int.zero;
    }

    private int GetElevationAtGridPosition(Vector2Int position) {
        Vector2 noiseSamplePosition = GridToWorldPosition(position) * elevationNoiseScale;
        float noiseValue = Mathf.Clamp01(Mathf.PerlinNoise(noiseSamplePosition.x, noiseSamplePosition.y));
        if (noiseValue <= oceansBoundary) {
            return 0;
        } else if (noiseValue > oceansBoundary && noiseValue < moutainsBoundary) {
            return 1;
        } else {
            return 2;
        }
    }

    private int GetBiomeAtGridPosition(Vector2Int position) {
        return 0;
    }

    [Header("Debug")]
    public bool debug;
    private void OnDrawGizmos() {
        if (debug) {
            Random.InitState(seed);

            for (int y = 0; y < boardSize.y; y++) {
                for (int x = 0; x < boardSize.x; x++) {
                    float level = 0.333f * GetElevationAtGridPosition(new Vector2Int(x, y));                                        
                    Gizmos.color = new Color(level, level, level);
                    Gizmos.DrawCube(GridToWorldPosition(new Vector2Int(x, y)), Tile.SIZE);
                }
            }
        }
    }

}