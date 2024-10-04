using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject playerNames;

    [SerializeField] private DebounceButton startButton;
    [SerializeField] private DebounceButton readyButton;
    [SerializeField] private DebounceButton leaveButton;

    void Start()
    {
        startButton.debounce = Constants.LOBBY_BUTTON_COOLDOWN*3;
        startButton.onClick = StartGame;

        readyButton.debounce = Constants.LOBBY_BUTTON_COOLDOWN;
        readyButton.onClick = ReadyUp;

        leaveButton.debounce = Constants.LOBBY_BUTTON_COOLDOWN;
        leaveButton.onClick = LeaveLobby;

        LobbyManager.onEnterLobby += SetupLobby;
        LobbyManager.onLobbyUpdate += UpdateStatus;
        LobbyManager.onPlayerUpdate += UpdateStatus;

    }

    private void SetupLobby()
    {
        startButton.gameObject.SetActive(LobbyManager.isHost);
        UpdateStatus();
    }
    private void LeaveLobby()
    {
        LobbyManager.LeaveLobby();
    }

    private async void ReadyUp()
    {
        await LobbyManager.ReadyToggle();
        readyButton.text = LobbyManager.isReady ? "Unready" : "Ready Up";
    }
    
    private async void StartGame()
    {
        Debug.Log("Attempting Start");
        int playerCount = LobbyManager.currentLobby.Players.Count;
        int readyCount = LobbyManager.currentLobby.Players.Select(LobbyManager.GetData).Count(p => p.ready);
        
        bool startConditions = readyCount == playerCount
                                && Constants.LOBBY_MIN_PLAYERS <= playerCount;
        if(!startConditions)
            return;
        
        Debug.Log("Condition passed, starting game");
        LobbyManager.StartGame();
    }

    private void UpdateStatus()
    {
        Lobby lobby = LobbyManager.currentLobby;
        int index = 0;

        int readyCount = 0;

        foreach (HoverButton button in playerNames.GetComponentsInChildren<HoverButton>())
        {
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if (lobby.Players.Count > index)
            {
                var player = lobby.Players[index];

                if (PlayerIDHolder.TryGetPlayer(player.Id, out var currentPlayer))
                {
                    bool ready = LobbyManager.GetData(player).ready;
                    
                    text.color = ready ? Color.green : Color.red;

                    if (ready)
                    {
                        readyCount++;
                    }
                    
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

        int playerCount = LobbyManager.currentLobby.Players.Count;
        
        bool enableConditions = readyCount == playerCount
                                && Constants.LOBBY_MIN_PLAYERS <= playerCount;

        startButton.buttonEnabled = enableConditions;
    }
}
