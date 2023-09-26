using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private int radius = 2;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float playerCheckRadius = 0.5f;
    [SerializeField] private Color outlineColor;
    [SerializeField] private Color photophobicTileColor;
    [SerializeField] private Color photophilicTileColor;

    private Dictionary<Vector3Int, TileData> photophobicTileTable;
    private Dictionary<Vector3Int, TileData> photophilicTileTable;
    private Transform playerTransform;

    private enum TileState { Active, ToInactive, Inactive, ToActive }
    private class TileData
    {
        public Vector3Int position;
        public TileState tileState;
        public Coroutine coroutine;

        public TileData(Vector3Int position, TileState tileState)
        {
            this.position = position;
            this.tileState = TileState.Active;
            coroutine = null;
        }
    }

    private void Awake()
    {
        photophobicTileTable = new Dictionary<Vector3Int, TileData>();
        photophilicTileTable = new Dictionary<Vector3Int, TileData>();

        // Fill tables
        foreach (var position in indicatorTilemap.cellBounds.allPositionsWithin)
        {
            // If tile here is photophobic
            if (indicatorTilemap.GetTile(position) == photophobicTile)
            {
                // Add to table
                var tileData = new TileData(position, TileState.Active);
                tileData.coroutine = StartCoroutine(FadeInTile(tileData, fadeDuration));
                photophobicTileTable[position] = tileData;

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
                photophilicTileTable[position] = new TileData(position, TileState.Inactive);

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
        playerTransform = PlayerController.instance.transform;
    }

    private void FixedUpdate()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0f;
        UpdateTilesInRange(position);
    }

    private void UpdateTilesInRange(Vector3 lightPosition)
    {
        // Check photophobic tiles
        foreach (var position in photophobicTileTable.Keys)
        {
            var tileData = photophobicTileTable[position];

            // If player is far away and not obstructing
            var worldPos = indicatorTilemap.GetCellCenterWorld(position);
            if (Vector3.Distance(lightPosition, worldPos) > radius && !IsObstructed(worldPos))
            {
                switch (tileData.tileState)
                {
                    case TileState.Active:

                        // Do nothing, we good

                        break;
                    case TileState.ToActive:

                        // We still good

                        break;
                    case TileState.ToInactive:

                        // Stop routine
                        StopCoroutine(tileData.coroutine);

                        // Start transition to active
                        tileData.coroutine = StartCoroutine(FadeInTile(tileData, fadeDuration));

                        break;
                    case TileState.Inactive:

                        // Start transition to active
                        tileData.coroutine = StartCoroutine(FadeInTile(tileData, fadeDuration));

                        break;
                }
            }
            else
            {
                switch (tileData.tileState)
                {
                    case TileState.Active:

                        // Start transition to inactive
                        tileData.coroutine = StartCoroutine(FadeOutTile(tileData, fadeDuration));

                        break;
                    case TileState.ToActive:

                        // Stop routine
                        StopCoroutine(tileData.coroutine);

                        // Start transition to inactive
                        tileData.coroutine = StartCoroutine(FadeOutTile(tileData, fadeDuration));

                        break;
                    case TileState.ToInactive:

                        // Do nothing, we good

                        break;
                    case TileState.Inactive:

                        // Do nothing, we good

                        break;
                }
            }
        }

        // Check photophilic tiles
        foreach (var position in photophilicTileTable.Keys)
        {
            var tileData = photophilicTileTable[position];

            // If player is close and not obstructing
            var worldPos = indicatorTilemap.GetCellCenterWorld(position);
            if (Vector3.Distance(lightPosition, worldPos) <= radius && !IsObstructed(worldPos))
            {
                switch (tileData.tileState)
                {
                    case TileState.Active:

                        // Do nothing, we good

                        break;
                    case TileState.ToActive:

                        // We still good

                        break;
                    case TileState.ToInactive:

                        // Stop routine
                        StopCoroutine(tileData.coroutine);

                        // Start transition to active
                        tileData.coroutine = StartCoroutine(FadeInTile(tileData, fadeDuration));

                        break;
                    case TileState.Inactive:

                        // Start transition to active
                        tileData.coroutine = StartCoroutine(FadeInTile(tileData, fadeDuration));

                        break;
                }
            }
            else
            {
                switch (tileData.tileState)
                {
                    case TileState.Active:

                        // Start transition to inactive
                        tileData.coroutine = StartCoroutine(FadeOutTile(tileData, fadeDuration));

                        break;
                    case TileState.ToActive:

                        // Stop routine
                        StopCoroutine(tileData.coroutine);

                        // Start transition to inactive
                        tileData.coroutine = StartCoroutine(FadeOutTile(tileData, fadeDuration));

                        break;
                    case TileState.ToInactive:

                        // Do nothing, we good

                        break;
                    case TileState.Inactive:

                        // Do nothing, we good

                        break;
                }
            }
        }
    }

    private bool IsObstructed(Vector3 tileWorldPosition)
    {
        return Vector3.Distance(tileWorldPosition, playerTransform.position) <= playerCheckRadius;
    }

    private IEnumerator FadeInTile(TileData tileData, float duration)
    {
        // Set state
        tileData.tileState = TileState.ToActive;

        // Set tile to active
        wallTilemap.SetTile(tileData.position, wallTile);

        // Set end-points
        float startAlpha = 0f;
        float endAlpha = 1f;
        Color color = Color.white;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            wallTilemap.SetColor(tileData.position, color);

            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        wallTilemap.SetColor(tileData.position, color);

        // Update state
        tileData.tileState = TileState.Active;
    }

    private IEnumerator FadeOutTile(TileData tileData, float duration)
    {
        // Set state
        tileData.tileState = TileState.ToInactive;

        // Set tile to active
        wallTilemap.SetTile(tileData.position, wallTile);

        // Set end-points
        float startAlpha = 1f;
        float endAlpha = 0f;
        Color color = Color.white;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            wallTilemap.SetColor(tileData.position, color);

            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        wallTilemap.SetColor(tileData.position, color);

        // Set tile
        wallTilemap.SetTile(tileData.position, null);

        // Update state
        tileData.tileState = TileState.Inactive;
    }
}
