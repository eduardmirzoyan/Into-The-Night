using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHandler : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool triggeredOnce;

    private void Awake()
    {
        triggeredOnce = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Play sound
        AudioManager.instance.PlaySFX("Complete Level");

        // If speedrunning
        if (GameManager.instance.SpeedrunEnabled() && !triggeredOnce)
        {
            // Stop timer
            SpeedrunTimerUI.instance.StopTimer();
            triggeredOnce = true;
            return;
        }

        // Go to next level
        GameManager.instance.NextLevel();
    }
}
