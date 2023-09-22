using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Dynamic Data")]
    [SerializeField] private bool isDead;
    [SerializeField] private bool isInvincible;

    [Header("Settings")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float hitDuration;

    private Material originalMaterial;
    private Coroutine flashRoutine;

    private void Awake()
    {
        // Get ref
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Save original material
        originalMaterial = spriteRenderer.material;
    }

    public bool IsDead() => isDead;

    public void Kill()
    {
        // Do nothing
        if (isInvincible) return;

        // If not already hit
        if (!isDead)
        {
            // Play effect
            HitEffect();

            // Set flag
            isInvincible = true;
            isDead = true;
        }
    }

    public void Revive()
    {
        if (isDead)
        {
            // Reset state
            isInvincible = false;
            isDead = false;
        }
    }

    private void HitEffect()
    {
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        // Start the Coroutine, and store the reference for it.
        flashRoutine = StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        // Swap to the flashMaterial.
        spriteRenderer.material = flashMaterial;

        // Freeze time
        Time.timeScale = 0f;

        // Wait.
        yield return new WaitForSecondsRealtime(hitDuration);

        // Resume time
        Time.timeScale = 1f;

        // After the pause, swap back to the original material.
        spriteRenderer.material = originalMaterial;

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }
}
