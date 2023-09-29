using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LeverToggleTilemap : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap indicatorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap outlineTilemap;

    [Header("Outline Data")]
    [SerializeField] private GameObject toggleableTilePrefab;
    [SerializeField] private Sprite toggleSprite;
    [SerializeField] private RuleTile outlineTile;
    [SerializeField] private Color outlineColor;
    [SerializeField] private Vector2Int boundaryOffset = Vector2Int.one;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private PolygonCollider2D boundaryCollider;

    public Action<Color> onTileEnable;
    public Action<Color> onTileDisable;

    public static LeverToggleTilemap instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        UpdateBoundary();

        FindTiles();
    }

    private void UpdateBoundary()
    {
        // Create a child to hold boundary
        var boundary = new GameObject("Boundary");
        boundary.transform.parent = transform;
        boundaryCollider = boundary.AddComponent<PolygonCollider2D>();

        // Resize collider based on tilemap
        wallTilemap.CompressBounds();
        Bounds bounds = wallTilemap.localBounds;

        // Define the points of the collider's new shape based on the Tilemap's bounds.
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(bounds.min.x + boundaryOffset.x, bounds.min.y + boundaryOffset.y);
        points[1] = new Vector2(bounds.min.x + boundaryOffset.x, bounds.max.y - boundaryOffset.y);
        points[2] = new Vector2(bounds.max.x - boundaryOffset.x, bounds.max.y - boundaryOffset.y);
        points[3] = new Vector2(bounds.max.x - boundaryOffset.x, bounds.min.y + boundaryOffset.y);

        // Set the points for the Polygon Collider 2D.
        boundaryCollider.SetPath(0, points);
    }

    private void FindTiles()
    {
        // Find all toggleable tiles
        foreach (var position in indicatorTilemap.cellBounds.allPositionsWithin)
        {
            if (indicatorTilemap.GetSprite(position) == toggleSprite)
            {
                var worldPos = wallTilemap.GetCellCenterWorld(position);
                var color = indicatorTilemap.GetColor(position);
                Instantiate(toggleableTilePrefab, worldPos, Quaternion.identity, indicatorTilemap.transform).GetComponent<ToggleableTile>().Initialize(position, color, outlineTile, wallTilemap, indicatorTilemap);

                outlineTilemap.SetTile(position, outlineTile);
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

    public Collider2D GetBoundingCollider()
    {
        return boundaryCollider;
    }
}
