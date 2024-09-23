using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;

/*
 * Handles initalizing and updating player states
 * while in the lobby
 */
public class LobbyLoader : MonoBehaviour
{
    [SerializeField] private GameObject playerHolder;
    [SerializeField] private GameObject playerNameHolder;
    [SerializeField] private TMP_Text lobbyName;

    private static LobbyLoader instance;

    private void Awake()
    {
        instance = this;
    }

    public static void UpdateLobby()
    {
        var lobby = LobbyManager.currentLobby;
        instance.lobbyName.text = lobby.Name;
        int index = 0;
        GameObject nameHolder = instance.playerNameHolder;
        foreach(TMP_Text text in instance.playerNameHolder.GetComponentsInChildren<TMP_Text>())
        {
            if(lobby.Players.Count > index)
            {
                var player = lobby.Players[index];
                var info = LobbyManager.GetData(player);
                text.text = info.name;
            }
            else
            {
                text.text = string.Empty;
            }
            index++;
        }
        foreach(LobbyPlayer player in lobby.Players)
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
