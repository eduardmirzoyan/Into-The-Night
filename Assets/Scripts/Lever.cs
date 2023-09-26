using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private SpriteRenderer leverRenderer;
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private Animator indicatorAnimator;

    [Header("On State")]
    [SerializeField] private Sprite onLeverSprite;
    [SerializeField] private Sprite onIndicatorSprite;
    [SerializeField] private Vector3 onColliderPosition = new Vector3(-0.7f, -0.35f, 0f);

    [Header("Off state")]
    [SerializeField] private Sprite offLeverSprite;
    [SerializeField] private Sprite offIndicatorSprite;
    [SerializeField] private Vector3 offColliderPosition = new Vector3(0.7f, -0.1f, 0f);

    [Header("Data")]
    [SerializeField, ReadOnly] private bool onState = true;

    private void Start()
    {
        // Always start on
        onState = true;
        leverRenderer.sprite = onLeverSprite;
        indicatorRenderer.sprite = onIndicatorSprite;
        collider2d.transform.localPosition = onColliderPosition;
        indicatorAnimator.Play("Active");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check direction
        var direction = collider2d.bounds.center - other.transform.position;
        direction.Normalize();
        if (onState && direction.x > 0f)
        {
            ToggleState();
        }
        else if (!onState && direction.x < 0f)
        {
            ToggleState();
        }
    }

    private void ToggleState()
    {
        if (onState)
        {
            leverRenderer.sprite = offLeverSprite;
            indicatorRenderer.sprite = offIndicatorSprite;
            indicatorAnimator.Play("Inactive");
            collider2d.transform.localPosition = offColliderPosition;

            LeverToggleTilemap.instance.DisableTiles(indicatorRenderer.color);

            onState = false;
        }
        else
        {
            leverRenderer.sprite = onLeverSprite;
            indicatorRenderer.sprite = onIndicatorSprite;
            indicatorAnimator.Play("Active");
            collider2d.transform.localPosition = onColliderPosition;


            LeverToggleTilemap.instance.EnableTiles(indicatorRenderer.color);

            onState = true;
        }

        // Play sound
        AudioManager.instance.PlaySFX("Lever");

        // Shake screen
        CameraShake.instance.ScreenShake(3f, 0.15f);
    }
}
