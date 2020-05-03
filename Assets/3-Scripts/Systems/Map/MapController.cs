﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    [System.Serializable]
    public struct BiomeSet {
        public string name;
        public Sprite[] sprites;
    }

    public int seed;

    public Transform tileContainer;
    public GameObject[] tilePrefabs;

    private HashSet<GameObject> tileObjects = new HashSet<GameObject>();

    public Vector2Int boardSize;
    public float noiseScale = 5f;

    private void Start() {
        Random.InitState(seed);

        for (int y = 0; y < boardSize.y; y++) {
            for (int x = 0; x < boardSize.x; x++) {
                Vector2Int gridPos = new Vector2Int(x, y);
                int elavationIndex = Mathf.FloorToInt(GetElevationAtGridPosition(gridPos) * tilePrefabs.Length);
                GameObject tileClone = Instantiate(tilePrefabs[elavationIndex], GridToWorldPosition(gridPos), Quaternion.identity);
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

    private float GetElevationAtGridPosition(Vector2Int position) {
        Vector2 noiseSamplePosition = GridToWorldPosition(position) * noiseScale;
        return Mathf.Clamp01(Mathf.PerlinNoise(noiseSamplePosition.x, noiseSamplePosition.y));
    }

    public bool debug;
    private void OnDrawGizmos() {
        if (debug) {
            Random.InitState(seed);

            for (int y = 0; y < boardSize.y; y++) {
                for (int x = 0; x < boardSize.x; x++) {
                    float level = GetElevationAtGridPosition(new Vector2Int(x, y));
                    Gizmos.color = new Color(level, level, level);
                    Gizmos.DrawSphere(GridToWorldPosition(new Vector2Int(x, y)), Tile.SIZE.x / 2);
                }
            }
        }
    }

}