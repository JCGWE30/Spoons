using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class GoldCaller : MonoBehaviour, ISkinCaller
{
    public async Task<bool> canEquip(string playerId)
    {
        try
        {
            LeaderboardEntry score = await LeaderboardsService.Instance.GetPlayerScoreAsync(Constants.LEADERBOARD_ID);
            return score.Rank == 0 ;
        }catch(LeaderboardsException e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public GameObject onDeath(GameObject deathObject)
    {
        return deathObject;
    }
}
