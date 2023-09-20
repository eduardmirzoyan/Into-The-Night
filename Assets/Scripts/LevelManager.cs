using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerController player;

    [Header("Settings")]
    [SerializeField] private Vector2Int mapSize;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    public static LevelManager instance;

    private void Awake()
    {
        // Singleton logic
        if (LevelManager.instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // Look for important game objects
        FindPlayer();

        // Start level after scene is opened
        // StartCoroutine(DelayedStart(TransitionManager.instance.GetTransitionTime()));
    }

    private IEnumerator DelayedStart(float duration)
    {
        // Wait a few
        yield return new WaitForSeconds(duration);

        // Open scene
        TransitionManager.instance.OpenScene(player.transform.position);

        // Trigger event
        LevelEvents.instance.TriggerOnLevelEnter(player.transform);
    }

    public Transform GetPlayerTransform() => player.transform;

    public void ExitLevel()
    {
        // Trigger event
        LevelEvents.instance.TriggerOnLevelExit(player.transform);
    }

    private void FindPlayer()
    {
        // Find player
        var player = GameObject.FindObjectOfType<PlayerController>();

        // Cache ref
        this.player = player;
    }

}
