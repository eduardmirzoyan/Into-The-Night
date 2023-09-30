using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathCounterUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField, ReadOnly] private TextMeshPro deathText;

    [Header("Settings")]
    [SerializeField] private Color defaultColor;

    public static DeathCounterUI instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        deathText = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        // If game has no counter
        if (!GameManager.instance.DeathCounterEnabled())
        {
            Destroy(gameObject);
            return;
        }

        int count = GameManager.instance.GetCurrentRestarts();
        deathText.text = $"Deaths: <color=yellow>{count}";
        StartCoroutine(Fade(1f, 1f));
    }

    private IEnumerator Fade(float fadeDuration, float pauseDuration)
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            Color color = Color.Lerp(Color.clear, defaultColor, elapsed / fadeDuration);
            deathText.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(pauseDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            Color color = Color.Lerp(defaultColor, Color.clear, elapsed / fadeDuration);
            deathText.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy when done
        Destroy(gameObject);
    }
}
