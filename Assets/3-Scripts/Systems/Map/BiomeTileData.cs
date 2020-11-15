using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeTileData : MonoBehaviour
{
    /// <summary> The constant size of all tiles in the game in unity units, used for positioning/laying out tiles. </summary>
    public static readonly Vector2 SIZE = new Vector2(1.2f, 1.38f);

    public enum Visibility
    {
        HIDDEN,
        DISCOVERED,
        VISIBLE
    }

    public enum Biome
    {
        ARCTIC,
        TUNDRA,
        GRSSLAND,
        FOREST,
        MOUNTAIN,
        DESERT,
        OCEAN
    }

    public enum Terrain
    {
        ROUGH,
        OPEN
    }

    public enum Walkable
    {
        YES,
        NO,
        YES_ON_BOUNDARY
    }

    public Biome biome;
    public Terrain terrain;
    public Walkable walkable;

    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public BiomeTileData[] adjacentTiles;
    [HideInInspector] public bool occupided;

    public Sprite primaryTile;
    public Sprite boundaryTile;

    private void Start()
    {

    }


}