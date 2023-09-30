using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dan.Main;

public class LeaderboardManager
{
    private static string PUBLIC_KEY = "a26b55152c62d532646911c7a54914fa5388077d048d8d0862d8f727adb73565";

    public List<string> GetEntries()
    {
        List<string> leaderboardEntries = new List<string>();

        LeaderboardCreator.GetLeaderboard(PUBLIC_KEY, (entries) =>
        {
            foreach (var entry in entries)
            {
                Debug.Log($"{entry.Rank}. {entry.Username} : {entry.Score}");

                string score = SpeedrunTimerUI.GetTimeAsString(entry.Score / 99f);

                string formatedEntry = $"{entry.Rank}. {entry.Username} : {score}";

                leaderboardEntries.Add(formatedEntry);
            }
        });

        Debug.Log(leaderboardEntries.Count);
        return leaderboardEntries;
    }

    public void UpdateScore(string username, float time)
    {
        int timeInMilli = (int)(time * 99f);

        Debug.Log($"Added user {username} with score: {timeInMilli}");
        LeaderboardCreator.UploadNewEntry(PUBLIC_KEY, username, timeInMilli, (_) =>
        {
            LeaderboardCreator.ResetPlayer();
        });
    }
}
