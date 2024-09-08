using System.Collections;
using System.Collections.Generic;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    private static LeaderboardManager instance;
    void Start()
    {
        instance = this;
    }

    public static async void UpdateScore()
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(Constants.LEADERBOARD_ID, 1);
    }
}
