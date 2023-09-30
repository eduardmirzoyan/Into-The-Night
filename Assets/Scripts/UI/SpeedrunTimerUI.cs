using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedrunTimerUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI minutesText;
    [SerializeField] private TextMeshProUGUI secondsText;
    [SerializeField] private TextMeshProUGUI milliText;

    [Header("Data")]
    [SerializeField, ReadOnly] private float startTime;
    [SerializeField, ReadOnly] private bool started;

    [Header("Settings")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color stoppedColor;
    [SerializeField] protected bool uploadToLeaderboard;

    private float time;
    private LeaderboardManager leaderboardManager;

    public static SpeedrunTimerUI instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        leaderboardManager = new LeaderboardManager();

        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
        // If game has no timer
        if (!GameManager.instance.SpeedrunEnabled())
        {
            Destroy(gameObject);
            return;
        }

        minutesText.color = Color.white;
        secondsText.color = Color.white;
        milliText.color = Color.white;

        // Start time at 0
        time = 0f;

        // Start
        StartTimer();
    }

    private void Update()
    {
        if (started)
        {
            time = Time.time - startTime;

            // Display
            UpdateDisplay(time);
        }
    }

    private void UpdateDisplay(float time)
    {
        int minutes = (int)(time / 60);
        minutesText.text = minutes + "'";

        int seconds = (int)(time % 60);
        secondsText.text = seconds + "''";

        float milliSecondsFloat = ((time % 60) - seconds) * 99;
        int milliSeconds = (int)Mathf.Round(milliSecondsFloat);

        string text = (milliSeconds < 10) ? "0" + milliSeconds : "" + milliSeconds;
        milliText.text = text + "'''";
    }

    private void StartTimer()
    {
        // Set start time
        startTime = Time.time;

        // Enable flag
        started = true;

        // Fade in timer
        StartCoroutine(FadeIn(0.5f));
    }

    public void StopTimer()
    {
        // Disable flag
        started = false;

        // Turn off color
        minutesText.color = stoppedColor;
        secondsText.color = stoppedColor;
        milliText.color = stoppedColor;


        if (uploadToLeaderboard)
        {
            // Update leaderboard
            string username = GameManager.instance.GetPlayerName();
            leaderboardManager.UpdateScore(username, time);
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        yield return new WaitForSeconds(2f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            Color color = Color.Lerp(Color.white, defaultColor, elapsed / duration);
            minutesText.color = color;
            secondsText.color = color;
            milliText.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public static string GetTimeAsString(float time)
    {
        string result = "";

        int minutes = (int)(time / 60);
        result += minutes + "' ";

        int seconds = (int)(time % 60);
        result += seconds + "'' ";

        float milliSecondsFloat = ((time % 60) - seconds) * 100;
        int milliSeconds = (int)Mathf.Round(milliSecondsFloat);

        string text = (milliSeconds < 10) ? "0" + milliSeconds : "" + milliSeconds;
        result += text + "'''";

        return result;
    }
}
