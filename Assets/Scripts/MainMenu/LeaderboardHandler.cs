using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardHandler : MonoBehaviour
{
    [SerializeField] private Image nameParent;

    private static LeaderboardHandler instance;

    private void Start()
    {
        instance = this;
        ServicesHandler.onServiceStart += UpdateLeaderboard;
    }

    private async void UpdateLeaderboard()
    {
        LeaderboardScoresPage scores = await LeaderboardsService.Instance.GetScoresAsync(Constants.LEADERBOARD_ID);
        int index = 0;
        foreach (Transform item in nameParent.transform)
        {
            if (scores.Results.Count <= index)
            {
                item.GetComponent<TMP_Text>().text = "";
                continue;
            }
            LeaderboardEntry score = scores.Results[index];
            item.GetComponent<TMP_Text>().text = score.PlayerName.Split("#")[0] + " - " + score.Score;
            index++;
        }
    }

}
