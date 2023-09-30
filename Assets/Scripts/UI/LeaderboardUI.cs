using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;
using Dan.Models;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI leaderboardText;

    [Header("Settings")]
    [SerializeField] private int numEntriesToDisplay = 10;

    private static string PUBLIC_KEY = "a26b55152c62d532646911c7a54914fa5388077d048d8d0862d8f727adb73565";

    public void UpdateDisplay()
    {
        LeaderboardCreator.GetLeaderboard(PUBLIC_KEY, DisplayEntries);
    }

    private void DisplayEntries(Entry[] entries)
    {
        string result = "";
        for (int i = 0; i < numEntriesToDisplay; i++)
        {
            // If we don't have enough entries
            if (i > entries.Length - 1)
            {
                break;
            }

            string score = SpeedrunTimerUI.GetTimeAsString(entries[i].Score / 99f);

            string formatedEntry = $"{entries[i].Rank}. {entries[i].Username} - <color=yellow>{score}</color>";

            result += formatedEntry + "\n";
        }

        leaderboardText.text = result;
    }
}
