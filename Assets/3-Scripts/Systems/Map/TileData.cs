using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {
    /// <summary> The constant size of all tiles in the game in unity units, used for positioning/laying out tiles. </summary>
    public static readonly Vector2 SIZE = new Vector2(1.2f, 1.38f);

    public enum Visibility {
        HIDDEN,
        DISCOVERED,
        VISIBLE
    }

    public enum Type {
        GRASS,
        FLOWERS,
        FOREST,
        LOGGING,
        FARM,
        VILLAGE
    }

    public Type type;
    private Visibility visibility;

    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public TileData[] adjacentTiles;

    private void Start() {

    }


}