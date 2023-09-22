using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Dynamic Data")]
    [SerializeField] private DamageHandler player;
    [SerializeField] private Vector3 currentCheckpoint;

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
        // Find player
        player = FindObjectOfType<PlayerController>().GetComponent<DamageHandler>();

        // Set current checkpoint
        currentCheckpoint = FindObjectOfType<EntranceHandler>().transform.position;

        // Start level after scene is opened
        StartCoroutine(DelayedStart(0.5f));
    }

    private IEnumerator DelayedStart(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Open scene
        TransitionManager.instance.OpenScene();

        // Trigger event
        LevelEvents.instance.TriggerOnLevelEnter();
    }

    private IEnumerator DelayedRestart(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Trigger event
        LevelEvents.instance.TriggerOnLevelRestart();

        // Open scene
        TransitionManager.instance.ReloadScene();
    }

    private IEnumerator DelayedRespawn(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Relocate player to last checkpoint
        player.transform.position = currentCheckpoint;
        player.Revive();
    }

    public void UpdateCheckpoint(Vector3 location)
    {
        // Save position
        currentCheckpoint = location;
    }

    public void Respawn()
    {
        StartCoroutine(DelayedRespawn(1f));
    }

    public void ExitLevel()
    {
        // Trigger event
        LevelEvents.instance.TriggerOnLevelExit();

        // Open scene
        TransitionManager.instance.LoadNextScene();
    }
}
