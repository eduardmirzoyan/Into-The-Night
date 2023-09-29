using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ToggleableTile : MonoBehaviour
{

    [Header("Components")]
    [SerializeField, ReadOnly] private RuleTile wallTile;
    [SerializeField, ReadOnly] private Tilemap wallTilemap;
    [SerializeField, ReadOnly] private Tilemap indicatorTilemap;

    [Header("Settings")]
    [SerializeField] private Tile enabledTile;
    [SerializeField] private Tile disabledTile;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private Vector3Int position;
    [SerializeField, ReadOnly] private Color color;
    [SerializeField, ReadOnly] private bool isActive;
    [SerializeField, ReadOnly] private int physicalCount;

    public void Initialize(Vector3Int position, Color color, RuleTile wallTile, Tilemap wallTilemap, Tilemap indicatorTilemap)
    {
        this.position = position;
        this.color = color;
        this.wallTile = wallTile;
        this.wallTilemap = wallTilemap;
        this.indicatorTilemap = indicatorTilemap;
        physicalCount = 0;

        // Start active
        ActivateOnEnable(color);

        // Update name
        gameObject.name = $"Toggleable Tile [{position}]";

        // Sub to events
        LeverToggleTilemap.instance.onTileEnable += ActivateOnEnable;
        LeverToggleTilemap.instance.onTileDisable += DeactivateOnDisable;
    }

    private void OnDestroy()
    {
        // Unsub to events
        LeverToggleTilemap.instance.onTileEnable -= ActivateOnEnable;
        LeverToggleTilemap.instance.onTileDisable -= DeactivateOnDisable;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if sometimes collides
        physicalCount++;

        // Assume we can't collide if active so we don't need to handle that case
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        physicalCount--;

        // If collision stopped and tile is supposed to be active, then turn on
        if (physicalCount == 0 && isActive)
        {
            isActive = false;
            ActivateOnEnable(color);
        }
    }

    private void ActivateOnEnable(Color color)
    {
        if (this.color == color)
        {
            // If inactive
            if (!isActive)
            {
                // If unobtructed
                if (physicalCount == 0)
                {
                    // Add tile
                    wallTilemap.SetTile(position, wallTile);

                    // Update indicator
                    indicatorTilemap.SetColor(position, color);
                    indicatorTilemap.SetTile(position, enabledTile);
                }

                isActive = true;
            }
        }
    }

    private void DeactivateOnDisable(Color color)
    {
        if (this.color == color)
        {
            if (isActive)
            {
                // Remove tile
                wallTilemap.SetTile(position, null);

                // Update indicator
                indicatorTilemap.SetColor(position, color);
                indicatorTilemap.SetTile(position, disabledTile);

                isActive = false;
            }
        }
    }
}
