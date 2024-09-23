using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyObjectManager : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text playerCount;
    [SerializeField] private DebounceButton joinButton;

    private Lobby lobby;

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyName.text = lobby.Name;
        playerCount.text = lobby.Players.Count.ToString();

        joinButton.debounce = Constants.LOBBY_UPDATE_COOLDOWN;

        joinButton.onClick = JoinLobby;
    }

    private async void JoinLobby()
    {
        bool success = await LobbyManager.JoinLobby(lobby);

        if (success)
        {
            MenuTransition.StartMove(TransitionMenu.LobbyMenu, 1f);
            LobbyLoader.UpdateLobby();
        }
    }
}
