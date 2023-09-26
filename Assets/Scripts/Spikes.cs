using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("triggered!");

        if (other.transform.parent.TryGetComponent(out DamageHandler damageHandler))
        {
            // Play sound
            AudioManager.instance.PlaySFX("Spikes");

            // Kill entity
            damageHandler.Kill();
        }
    }
}
