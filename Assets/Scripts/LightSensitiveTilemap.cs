using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightSensitiveTilemap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private Tilemap indicatorTilemap;
    [SerializeField] private Tilemap outlineTilemap;
    [SerializeField] private Tile photophobicTile;
    [SerializeField] private Tile photophilicTile;

    [Header("Settings")]
    [SerializeField] private float innerRadius = 1f;
    [SerializeField] private float outerRadius = 2f;
    [SerializeField] private Color outlineColor;
    [SerializeField] private Color photophobicTileColor;
    [SerializeField] private Color photophilicTileColor;

    private Dictionary<Vector3Int, bool> photophobicTileTable;
    private Dictionary<Vector3Int, bool> photophilicTileTable;
    private Transform playerTransform;

    private void Awake()
    {
        photophobicTileTable = new Dictionary<Vector3Int, bool>();
        photophilicTileTable = new Dictionary<Vector3Int, bool>();

        // Fill tables
        foreach (var position in indicatorTilemap.cellBounds.allPositionsWithin)
        {
            // If tile here is photophobic
            if (indicatorTilemap.GetTile(position) == photophobicTile)
            {
                // Add to table
                photophobicTileTable[position] = true;

                // Update color of indicator
                indicatorTilemap.SetColor(position, photophobicTileColor);

                // Turn off color lock
                wallTilemap.SetTileFlags(position, TileFlags.None);

                // Set outline
                outlineTilemap.SetTileFlags(position, TileFlags.None);
                outlineTilemap.SetTile(position, wallTile);
                outlineTilemap.SetColor(position, outlineColor);
            }
            // If tile here is photophilic
            else if (indicatorTilemap.GetTile(position) == photophilicTile)
            {
                // Add to table
                photophilicTileTable[position] = true;

                // Update color of indicator
                indicatorTilemap.SetColor(position, photophilicTileColor);

                // Turn off color lock
                wallTilemap.SetTileFlags(position, TileFlags.None);

                // Set outline
                outlineTilemap.SetTileFlags(position, TileFlags.None);
                outlineTilemap.SetTile(position, wallTile);
                outlineTilemap.SetColor(position, outlineColor);
            }
        }
    }

    private void Start()
    {
        // Find player
        playerTransform = FindObjectOfType<PlayerController>().transform;
    }

    private void FixedUpdate()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0f;
        UpdateLightSensitiveTiles(position);
    }

    private void UpdateLightSensitiveTiles(Vector3 lightPosition)
    {
        // Check photophilic tiles
        foreach (var position in photophilicTileTable.Keys)
        {
            var worldPos = indicatorTilemap.GetCellCenterWorld(position);
            float distance = Vector3.Distance(lightPosition, worldPos);
            if (distance <= outerRadius && !IsObstructed(worldPos))
            {
                // Set transparancy based on distance
                distance = Mathf.Clamp(distance, innerRadius, outerRadius);
                float t = Mathf.InverseLerp(innerRadius, outerRadius, distance);
                Color color = Color.Lerp(Color.white, Color.clear, t);

                wallTilemap.SetColor(position, color);
                wallTilemap.SetTile(position, wallTile);
            }
            else
            {
                wallTilemap.SetTile(position, null);
            }
        }

        // Check photophobic tiles
        foreach (var position in photophobicTileTable.Keys)
        {
            var worldPos = indicatorTilemap.GetCellCenterWorld(position);
            float distance = Vector3.Distance(lightPosition, worldPos);
            if (distance > outerRadius && !IsObstructed(worldPos))
            {
                wallTilemap.SetTile(position, wallTile);
            }
            else
            {
                wallTilemap.SetTile(position, null);
            }
        }
    }

    private bool IsObstructed(Vector3 tileWorldPosition)
    {
        return Vector3.Distance(tileWorldPosition, playerTransform.position) <= 0.5f;
    }
}
