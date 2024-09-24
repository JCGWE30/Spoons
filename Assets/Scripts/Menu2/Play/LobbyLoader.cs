using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        LobbyManager.onLobbyUpdate += UpdateLobby;
    }

    public static void UpdateLobby()
    {
        var lobby = LobbyManager.currentLobby;
        instance.lobbyName.text = lobby.Name;

        GameObject nameHolder = instance.playerNameHolder;

        List<PlayerIDHolder> playerMarkers = instance.GetComponentsInChildren<PlayerIDHolder>().ToList();
        playerMarkers = playerMarkers.OrderBy(x => Random.Range(0, int.MaxValue)).ToList();

        int index = 0;
        foreach (PlayerIDHolder playerID in playerMarkers)
        {
            var player = lobby.Players[index];
            if (PlayerIDHolder.TryGetPlayer(player.Id, out var currentPlayer))
            {
                currentPlayer.Set(player);
            }
            else
            {
                playerID.Set(player);
            }
            index++;
        }

        foreach (HoverButton button in instance.playerNameHolder.GetComponentsInChildren<HoverButton>())
        {
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if(lobby.Players.Count > index)
            {
                var player = lobby.Players[index];

                if(PlayerIDHolder.TryGetPlayer(player.Id,out var currentPlayer))
                {
                    button.onHover = () => currentPlayer.SetHighlightState(true);
                    button.onUnHover = () => currentPlayer.SetHighlightState(false);
                }
                else
                {
                    button.Wipe();
                }

                var info = LobbyManager.GetData(player);
                text.text = info.name;
            }
            else
            {
                text.text = string.Empty;
            }
            index++;
        }
    }
}
