using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Play sound
        AudioManager.instance.PlaySFX("Complete Level");

        // Go to next level
        GameManager.instance.NextLevel();
    }
}
