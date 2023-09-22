using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Exit level
        LevelManager.instance.ExitLevel();
    }
}
