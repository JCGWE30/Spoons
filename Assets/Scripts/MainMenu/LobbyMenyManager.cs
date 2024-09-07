using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerPrefab;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbyCode;
    [SerializeField] private Image playerPanel;

    [SerializeField] private Button startGame;
    [SerializeField] private Button leaveLobby;

    private void Start()
    {
        LobbyHandler.onUpdate += UpdateLobby;
        startGame.onClick.AddListener(delegate { StartGame(); });
        leaveLobby.onClick.AddListener(delegate { LeaveLobby(); });
    }

    private void UpdateLobby(Lobby lobby)
    {
        foreach (Transform item in playerPanel.gameObject.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (var player in lobby.Players)
        {
            TMP_Text playerName = Instantiate(playerPrefab);
            playerName.text = player.Data[Constants.KEY_PLAYER_NAME].Value;
            playerName.transform.parent = playerPanel.transform;
        }
        lobbyName.text = lobby.Name;
        lobbyCode.text = "Code: " + lobby.LobbyCode;
    }

    private void StartGame()
    {
        LobbyHandler.StartGame();
    }

    private void LeaveLobby()
    {
        LobbyHandler.LeaveLobby();
    }
}
