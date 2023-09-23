using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private HitFlash hitFlash;

    [Header("Dynamic Data")]
    [SerializeField] private bool isDead;
    [SerializeField] private bool isInvincible;

    private void Awake()
    {
        hitFlash = GetComponent<HitFlash>();
    }

    public void Kill()
    {
        // Do nothing
        if (isInvincible) return;

        // If not already hit
        if (!isDead)
        {
            // Play effect
            if (hitFlash != null)
            {
                hitFlash.Flash();
            }

            // Set flag
            isInvincible = true;
            isDead = true;
        }
    }

    public bool IsDead()
    {
        return isDead;
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

}
