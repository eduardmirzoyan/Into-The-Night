using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelEvents : MonoBehaviour
{
    public static LevelEvents instance;
    private void Awake()
    {
        // Singleton Logic
        if (LevelEvents.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public event Action<Transform> onLevelEnter;
    public event Action onPlayerDeath;
    public event Action<Transform> onLevelExit;

    public void TriggerOnLevelEnter(Transform playerTransform)
    {
        if (onLevelEnter != null)
        {
            onLevelEnter(playerTransform);
        }
    }

    public void TriggerOnPlayerDeath()
    {
        if (onPlayerDeath != null)
        {
            onPlayerDeath();
        }
    }

    public void TriggerOnLevelExit(Transform playerTransform)
    {
        if (onLevelExit != null)
        {
            onLevelExit(playerTransform);
        }
    }
}
