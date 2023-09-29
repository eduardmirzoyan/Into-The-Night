using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightAlter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private Animator indicatorAnimator;
    [SerializeField] private Light2D light2d;

    [Header("On State")]
    [SerializeField] private Sprite onIndicatorSprite;

    [Header("Off state")]
    [SerializeField] private Sprite offIndicatorSprite;

    [Header("Data")]
    [SerializeField, ReadOnly] private int triggerCount;

    private void Awake()
    {
        light2d = GetComponentInChildren<Light2D>();
        light2d.color = indicatorRenderer.color;
    }

    private void Start()
    {
        // Start disabled
        LeverToggleTilemap.instance.DisableTiles(indicatorRenderer.color);

        indicatorRenderer.sprite = offIndicatorSprite;
        indicatorAnimator.Play("Inactive");
        triggerCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enable tiles lol
        // print($"Enabled by: {other.name}");

        triggerCount++;
        if (triggerCount == 1)
        {
            LeverToggleTilemap.instance.EnableTiles(indicatorRenderer.color);

            // Play sound
            AudioManager.instance.PlaySFX("Alter On");

            indicatorRenderer.sprite = onIndicatorSprite;
            indicatorAnimator.Play("Active");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Disable tiles lol
        // print($"Disabled by: {other.name}");

        triggerCount--;
        if (triggerCount == 0)
        {
            LeverToggleTilemap.instance.DisableTiles(indicatorRenderer.color);

            // Play sound
            AudioManager.instance.PlaySFX("Alter Off");

            indicatorRenderer.sprite = offIndicatorSprite;
            indicatorAnimator.Play("Inactive");
        }
    }
}
