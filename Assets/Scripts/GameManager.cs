using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Settings Data")]
    [SerializeField,] private bool enableSpeedrunTimer;
    [SerializeField,] private bool enableDeathCounter;
    [SerializeField,] private string playerName;

    [Header("Level Data")]
    [SerializeField, ReadOnly] private int restartCount;
    [SerializeField, ReadOnly] private int currentLevel;
    [SerializeField, ReadOnly] private Vector3 currentCheckpoint;

    public static GameManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this);

        // Set to default values
        restartCount = 0;
        currentLevel = 0;
        currentCheckpoint = Vector3.back;
        playerName = "";
    }

    public void EnableSpeedrunTimer(bool state)
    {
        enableSpeedrunTimer = state;
    }

    public bool SpeedrunEnabled()
    {
        return enableSpeedrunTimer;
    }

    public void EnableDeathCounter(bool state)
    {
        enableDeathCounter = state;
    }

    public bool DeathCounterEnabled()
    {
        return enableDeathCounter;
    }

    public void SetCheckpoint(Vector3 checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public Vector3 GetCurrentCheckpoint()
    {
        return currentCheckpoint;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetCurrentRestarts()
    {
        return restartCount;
    }

    public void LeaveLevel()
    {
        // Reset count
        restartCount = 0;

        // Reset checkpoint
        currentCheckpoint = Vector3.back;

        // Reset level
        currentLevel = 0;

        // Load main menu
        TransitionManager.instance.LoadSelectedScene(currentLevel);
    }

    public void RestartLevel()
    {
        // Increment count
        restartCount++;

        // Reload same scene
        TransitionManager.instance.ReloadScene();
    }

    public void NextLevel()
    {
        // Reset count
        restartCount = 0;

        // Reset checkpoint
        currentCheckpoint = Vector3.back;

        // Increment level
        currentLevel++;

        // Open scene
        TransitionManager.instance.LoadSelectedScene(currentLevel);
    }

    public void EnterLevel(int index)
    {
        // Reset checkpoint
        currentCheckpoint = Vector3.back;

        // Set level
        currentLevel = index;

        // Open scene
        TransitionManager.instance.LoadSelectedScene(currentLevel);
    }

}
