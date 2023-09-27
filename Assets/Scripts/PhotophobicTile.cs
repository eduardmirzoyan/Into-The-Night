using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Hides from light
public class PhotophobicTile : MonoBehaviour
{
    private enum TileState { Active, ToInactive, Inactive, ToActive }

    [Header("Components")]
    [SerializeField, ReadOnly] private Tilemap wallTilemap;
    [SerializeField, ReadOnly] private RuleTile wallTile;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private string physicalLayer = "Physical";

    [Header("Debugging")]
    [SerializeField, ReadOnly] private Vector3Int position;
    [SerializeField, ReadOnly] private TileState tileState;
    [SerializeField, ReadOnly] private int lightCount;
    [SerializeField, ReadOnly] private int physicalCount;

    private Coroutine coroutine;

    public void Initialize(Vector3Int position, RuleTile wallTile, Tilemap wallTilemap)
    {
        this.position = position;
        this.wallTile = wallTile;
        this.wallTilemap = wallTilemap;
        lightCount = 0;
        physicalCount = 0;

        // Start active
        tileState = TileState.Active;
        wallTilemap.SetTile(position, wallTile);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If physical contact
        if (LayerMask.NameToLayer(physicalLayer) == other.gameObject.layer)
        {
            // Increment count
            physicalCount++;

            // Do nothing, we assume tile is deactivated to allow movement
        }
        else
        {
            // Increment count
            lightCount++;

            // Handle first time
            if (lightCount == 1)
            {
                Deactivate();
            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // If physical contact
        if (LayerMask.NameToLayer(physicalLayer) == other.gameObject.layer)
        {
            // Decrement count
            physicalCount--;

            // If no blocker and no light
            if (physicalCount == 0 && lightCount == 0)
            {
                Activate();
            }
        }
        else
        {
            // Decrement count
            lightCount--;

            // If no blocker and no light
            if (lightCount == 0 && physicalCount == 0)
            {
                Activate();
            }
        }
    }

    private void Activate()
    {
        switch (tileState)
        {
            case TileState.Active:

                // Do nothing, we good

                break;
            case TileState.ToActive:

                // We still good

                break;
            case TileState.ToInactive:

                // Stop routine
                StopCoroutine(coroutine);

                // Start transition to active
                coroutine = StartCoroutine(FadeIn(fadeDuration));

                break;
            case TileState.Inactive:

                // Start transition to active
                coroutine = StartCoroutine(FadeIn(fadeDuration));

                break;
        }
    }

    private void Deactivate()
    {
        switch (tileState)
        {
            case TileState.Active:

                // Start transition to inactive
                coroutine = StartCoroutine(FadeOut(fadeDuration));

                break;
            case TileState.ToActive:

                // Stop routine
                StopCoroutine(coroutine);

                // Start transition to inactive
                coroutine = StartCoroutine(FadeOut(fadeDuration));

                break;
            case TileState.ToInactive:

                // Do nothing, we good

                break;
            case TileState.Inactive:

                // Do nothing, we good

                break;
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        // Set state
        tileState = TileState.ToActive;

        // Set tile to active
        wallTilemap.SetTile(position, wallTile);

        // Set end-points
        float startAlpha = 0f;
        float endAlpha = 1f;
        Color color = Color.white;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            wallTilemap.SetColor(position, color);

            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        wallTilemap.SetColor(position, color);

        // Update state
        tileState = TileState.Active;

        // Reset routine
        coroutine = null;
    }

    private IEnumerator FadeOut(float duration)
    {
        // Set state
        tileState = TileState.ToInactive;

        // Set tile to active
        wallTilemap.SetTile(position, wallTile);

        // Set end-points
        float startAlpha = 1f;
        float endAlpha = 0f;
        Color color = Color.white;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            wallTilemap.SetColor(position, color);

            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        wallTilemap.SetColor(position, color);

        // Set tile
        wallTilemap.SetTile(position, null);

        // Update state
        tileState = TileState.Inactive;

        // Reset routine
        coroutine = null;
    }
}
