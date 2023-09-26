using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAlter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private Animator indicatorAnimator;

    [Header("On State")]
    [SerializeField] private Sprite onIndicatorSprite;

    [Header("Off state")]
    [SerializeField] private Sprite offIndicatorSprite;

    [Header("Data")]
    [SerializeField, ReadOnly] private bool onState;
    [SerializeField, ReadOnly] private int triggerCount;

    private void Start()
    {
        // Start disabled
        LeverToggleTilemap.instance.DisableTiles(indicatorRenderer.color);

        onState = false;
        indicatorRenderer.sprite = offIndicatorSprite;
        indicatorAnimator.Play("Inactive");
        triggerCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enable tiles lol
        print($"Enabled by: {other.name}");

        triggerCount++;
        if (triggerCount == 1)
        {
            LeverToggleTilemap.instance.EnableTiles(indicatorRenderer.color);

            // Play sound
            AudioManager.instance.PlaySFX("Alter On");

            onState = true;
            indicatorRenderer.sprite = onIndicatorSprite;
            indicatorAnimator.Play("Active");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Disable tiles lol
        print($"Disabled by: {other.name}");

        triggerCount--;
        if (triggerCount == 0)
        {
            LeverToggleTilemap.instance.DisableTiles(indicatorRenderer.color);

            // Play sound
            AudioManager.instance.PlaySFX("Alter Off");

            onState = false;
            indicatorRenderer.sprite = offIndicatorSprite;
            indicatorAnimator.Play("Inactive");
        }
    }
}
