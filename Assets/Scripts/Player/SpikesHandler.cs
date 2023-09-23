using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private DamageHandler damageHandler;

    private void Awake()
    {
        // Get refs
        damageHandler = GetComponent<DamageHandler>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Spikes _))
        {
            // Play sound
            AudioManager.instance.PlaySFX("Spikes");

            // Kill character
            damageHandler.Kill();
        }
    }


}
