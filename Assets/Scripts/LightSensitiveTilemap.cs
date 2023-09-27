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
    [SerializeField] private GameObject photophobicPrefab;
    [SerializeField] private GameObject photophilicPrefab;

    [Header("Settings")]
    [SerializeField] private int radius = 2;
    [SerializeField] private Color outlineColor;

    private void Awake()
    {
        // Fill tables
        foreach (var position in indicatorTilemap.cellBounds.allPositionsWithin)
        {
            // If tile here is photophobic
            if (indicatorTilemap.GetTile(position) == photophobicTile)
            {
                // Create object
                var worldPos = wallTilemap.GetCellCenterWorld(position);
                Instantiate(photophobicPrefab, worldPos, Quaternion.identity, indicatorTilemap.transform).GetComponent<PhotophobicTile>().Initialize(position, wallTile, wallTilemap);

                // Set outline
                outlineTilemap.SetTileFlags(position, TileFlags.None);
                outlineTilemap.SetTile(position, wallTile);
                outlineTilemap.SetColor(position, outlineColor);
            }
            // If tile here is photophilic
            else if (indicatorTilemap.GetTile(position) == photophilicTile)
            {
                // Create object
                var worldPos = wallTilemap.GetCellCenterWorld(position);
                Instantiate(photophilicPrefab, worldPos, Quaternion.identity, indicatorTilemap.transform).GetComponent<PhotophilicTile>().Initialize(position, wallTile, wallTilemap);

                // Set outline
                outlineTilemap.SetTileFlags(position, TileFlags.None);
                outlineTilemap.SetTile(position, wallTile);
                outlineTilemap.SetColor(position, outlineColor);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Gizmos.color = Color.red;
        for (float i = -radius; i <= radius; i++)
        {
            for (float j = -radius; j <= radius; j++)
            {
                var position = mousePosition + new Vector3(i, j, 0);
                var tilePosition = wallTilemap.WorldToCell(position);
                var tilePositionWorld = wallTilemap.GetCellCenterWorld(tilePosition);

                // Check if cell center and mouse are within radius
                if (Vector3.Distance(mousePosition, tilePositionWorld) <= radius)
                {
                    Gizmos.DrawWireCube(tilePositionWorld, Vector3.one);
                }
            }
        }
    }
}
