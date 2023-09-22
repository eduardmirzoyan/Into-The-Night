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

    public event Action onLevelEnter;
    public event Action onLevelRestart;
    public event Action onLevelExit;

    public void TriggerOnLevelEnter()
    {
        if (onLevelEnter != null)
        {
            onLevelEnter();
        }
    }

    public void TriggerOnLevelRestart()
    {
        if (onLevelRestart != null)
        {
            onLevelRestart();
        }
    }

    public void TriggerOnLevelExit()
    {
        if (onLevelExit != null)
        {
            onLevelExit();
        }
    }
}
