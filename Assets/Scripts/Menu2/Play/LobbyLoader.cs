using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using UnityEngine;
using System.Linq;

/*
 * Handles initalizing and updating player states
 * while in the lobby
 */
public class LobbyLoader : MonoBehaviour
{
    [SerializeField] private GameObject playerHolder;

    private static LobbyLoader instance;

    private void Start()
    {
        instance = this;
    }

    public static void UpdateLobby(Lobby lobby)
    {
        foreach(LobbyPlayer player in LobbyManager.currentLobby.Players)
        {
            if(PlayerIDHolder.TryGetPlayer(player.Id,out var currentPlayer))
            {
                currentPlayer.Set(player);
            }
            else
            {
                List<PlayerIDHolder> playerMarkers = instance.GetComponentsInChildren<PlayerIDHolder>().ToList();
                playerMarkers = playerMarkers.OrderBy(x => Random.Range(0, int.MaxValue)).ToList();
                playerMarkers.First().Set(player);
            }
        }
    }
}
