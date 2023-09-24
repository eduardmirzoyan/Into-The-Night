using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    [SerializeField] private Vector3 currentCheckpoint;

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
        currentLevel = 0;
        currentCheckpoint = Vector3.back;
    }

    public void SetCheckpoint(Vector3 checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public Vector3 GetCheckpoint()
    {
        return currentCheckpoint;
    }

    public void LeaveLevel()
    {
        // Reset checkpoint
        currentCheckpoint = Vector3.back;

        // Reset level
        currentLevel = 0;

        // Load main menu
        TransitionManager.instance.LoadSelectedScene(currentLevel);
    }

    public void RestartLevel()
    {
        // Reload same scene
        TransitionManager.instance.ReloadScene();
    }

    public void NextLevel()
    {
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
