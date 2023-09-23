using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LeverToggleTilemap : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap indicatorTilemap;
    [SerializeField] private Tile enabledTile;
    [SerializeField] private Tile disabledTile;

    [Header("Outline Data")]
    [SerializeField] private Tilemap outlineTilemap;
    [SerializeField] private RuleTile outlineTile;
    [SerializeField] private Color outlineColor;

    private Dictionary<Color, List<Vector3Int>> colorToTilesTable;

    public static LeverToggleTilemap instance;
    private void Awake()
    {
        // Singleton Logic
        if (LeverToggleTilemap.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        FindTiles();
    }

    private void FindTiles()
    {
        colorToTilesTable = new Dictionary<Color, List<Vector3Int>>();

        // Find all toggleable tiles
        foreach (var position in indicatorTilemap.cellBounds.allPositionsWithin)
        {
            if (indicatorTilemap.GetSprite(position) == enabledTile.sprite)
            {
                var color = indicatorTilemap.GetColor(position);
                if (colorToTilesTable.TryGetValue(color, out var positions))
                {
                    // Update entry
                    positions.Add(position);
                }
                else
                {
                    // Create new entry
                    colorToTilesTable[color] = new List<Vector3Int>() { position };
                }

                // Enable tile here
                wallTilemap.SetTile(position, outlineTile);

                outlineTilemap.SetTile(position, outlineTile);
                outlineTilemap.SetColor(position, outlineColor);
            }
        }
    }

    public void EnableTiles(Color color)
    {
        // If color is valid
        if (colorToTilesTable.TryGetValue(color, out var positions))
        {
            foreach (var position in positions)
            {
                // Enable tile
                wallTilemap.SetTile(position, outlineTile);

                // Update icon
                indicatorTilemap.SetTileFlags(position, TileFlags.None);
                indicatorTilemap.SetColor(position, color);
                indicatorTilemap.SetTile(position, enabledTile);
            }
        }
    }

    public void DisableTiles(Color color)
    {
        // If color is valid
        if (colorToTilesTable.TryGetValue(color, out var positions))
        {
            foreach (var position in positions)
            {
                // Disable tile
                wallTilemap.SetTile(position, null);

                // Update icon
                indicatorTilemap.SetTileFlags(position, TileFlags.None);
                indicatorTilemap.SetColor(position, color);
                indicatorTilemap.SetTile(position, disabledTile);
            }
        }
    }
}
