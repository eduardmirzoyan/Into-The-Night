using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CheckpointController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem sparkParticles;
    [SerializeField] private Light2D light2D;

    [Header("Debugging")]
    [SerializeField] private bool isActive;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        light2D = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        animator.Play("Inactive");
        light2D.intensity = 0f;

        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive)
        {
            // Update checkpoint
            LevelManager.instance.UpdateCheckpoint(other.transform.position);

            // Play animation
            animator.Play("Active");

            // Show particles
            fireParticles.Play();
            sparkParticles.Play();

            // Show light over time
            StartCoroutine(ShineLight(0.5f));

            isActive = true;
        }
    }

    private IEnumerator ShineLight(float duration)
    {
        float elapased = 0f;
        while (elapased < duration)
        {
            light2D.intensity = Mathf.Lerp(0f, 1f, elapased / duration);

            elapased += Time.deltaTime;
            yield return null;
        }

        light2D.intensity = 1f;
    }
}
