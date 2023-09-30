using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PhotoSensitiveTilesManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap indicatorTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap outlineTilemap;

    [Header("Tile Data")]
    [SerializeField] private RuleTile groundTile;
    [SerializeField] private Tile photophobicTile;
    [SerializeField] private Tile photophilicTile;
    [SerializeField] private GameObject photophobicPrefab;
    [SerializeField] private GameObject photophilicPrefab;

    [Header("Settings")]
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
                var worldPos = groundTilemap.GetCellCenterWorld(position);
                Instantiate(photophobicPrefab, worldPos, Quaternion.identity, indicatorTilemap.transform).GetComponent<PhotophobicTile>().Initialize(position, groundTile, groundTilemap);

                // Set outline
                outlineTilemap.SetTileFlags(position, TileFlags.None);
                outlineTilemap.SetTile(position, groundTile);
                outlineTilemap.SetColor(position, outlineColor);
            }
            // If tile here is photophilic
            else if (indicatorTilemap.GetTile(position) == photophilicTile)
            {
                // Create object
                var worldPos = groundTilemap.GetCellCenterWorld(position);
                Instantiate(photophilicPrefab, worldPos, Quaternion.identity, indicatorTilemap.transform).GetComponent<PhotophilicTile>().Initialize(position, groundTile, groundTilemap);

                // Set outline
                outlineTilemap.SetTileFlags(position, TileFlags.None);
                outlineTilemap.SetTile(position, groundTile);
                outlineTilemap.SetColor(position, outlineColor);
            }
        }
    }
}
