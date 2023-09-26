using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EntranceHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshPro displayText;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine coroutine;

    private void Awake()
    {
        displayText = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        // Text is set to level
        int level = GameManager.instance.GetCurrentLevel();
        displayText.text = $"Level {level}";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(FadeInInstructions(fadeDuration));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(FadeOutInstructions(fadeDuration));
    }

    private IEnumerator FadeInInstructions(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            displayText.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutInstructions(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            displayText.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
