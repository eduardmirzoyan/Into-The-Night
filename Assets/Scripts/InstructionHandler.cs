using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public class InstructionHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshPro instructionText;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Data")]
    [TextArea(4, 5)]
    [SerializeField] private string instructions;

    private Coroutine coroutine;

    private void Start()
    {
        instructionText.text = instructions;
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
            instructionText.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutInstructions(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            instructionText.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
