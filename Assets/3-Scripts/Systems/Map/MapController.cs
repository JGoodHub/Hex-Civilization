using System.Collections;
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

    private void Start() {
        Random.InitState(seed);

        for (int y = 0; y < boardSize.y; y++) {
            for (int x = 0; x < boardSize.x; x++) {
                GameObject tileClone = Instantiate(tilePrefabs[0], GridToWorldPosition(new Vector2(x, y)), Quaternion.identity);
                tileObjects.Add(tileClone);

                tileClone.transform.parent = tileContainer;
            }
        }
    }

    private Vector2 GridToWorldPosition(Vector2 position) {
        return new Vector2((position.x * Tile.SIZE.x) + (position.y % 2 * (Tile.SIZE.x / 2)), position.y * (Tile.SIZE.y * 0.75f));
    }

    private Vector2 WorldToGridPosition(Vector2 position) {
        return Vector2.zero;
    }
}