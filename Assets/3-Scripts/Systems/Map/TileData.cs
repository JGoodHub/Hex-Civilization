using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
    /// <summary> The constant size of all tiles in the game in unity units, used for positioning/laying out tiles. </summary>
    public static readonly Vector2 SIZE = new Vector2(1f, 1.155f);

    public enum Visibility
    {
        HIDDEN,
        DISCOVERED,
        VISIBLE
    }

    public enum Type
    {
        EMPTY = -1,
        OCEAN,
        GRASS,
        FLOWERS,
        FOREST,
        BEACH,
        MOUNTAIN,
        DESERT,
        VILLAGE,
        WHEAT_FARM,
        LOGGING
    }

    public Type type;
    private Visibility visibility;

    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public TileData[] adjacentTiles;

}