using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConditionalTilesManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap indicatorTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap outlineTilemap;

    [Header("Data")]
    [SerializeField] private RuleTile groundTile;
    [SerializeField] private Sprite toggleSprite;
    [SerializeField] private GameObject toggleableTilePrefab;

    [Header("Settings")]
    [SerializeField] private Color outlineColor;

    public Action<Color> onTileEnable;
    public Action<Color> onTileDisable;

    public static ConditionalTilesManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        FindTiles();
    }

    private void FindTiles()
    {
        if (indicatorTilemap == null)
            return;

        // Find all toggleable tiles
        foreach (var position in indicatorTilemap.cellBounds.allPositionsWithin)
        {
            if (indicatorTilemap.GetSprite(position) == toggleSprite)
            {
                var worldPos = groundTilemap.GetCellCenterWorld(position);
                var color = indicatorTilemap.GetColor(position);
                Instantiate(toggleableTilePrefab, worldPos, Quaternion.identity, indicatorTilemap.transform).GetComponent<ToggleableTile>().Initialize(position, color, groundTile, groundTilemap, indicatorTilemap);

                outlineTilemap.SetTile(position, groundTile);
                outlineTilemap.SetColor(position, outlineColor);
            }
        }
    }

    public void EnableTiles(Color color)
    {
        if (onTileEnable != null)
        {
            onTileEnable(color);
        }
    }

    public void DisableTiles(Color color)
    {
        if (onTileDisable != null)
        {
            onTileDisable(color);
        }
    }
}
