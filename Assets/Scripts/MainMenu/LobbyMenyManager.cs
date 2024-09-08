using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerPrefab;
    [SerializeField] private Image playerPanel;
    [SerializeField] private Button startGame;
    [SerializeField] private Button leaveLobby;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbyCode;
    [SerializeField] private Button instaKill;

    private void Start()
    {
        LobbyHandler.onUpdate += UpdateLobby;
        LobbyHandler.onEnterLobby += _ => ResetLobby();
        startGame.onClick.AddListener(delegate { StartGame(); });
        leaveLobby.onClick.AddListener(delegate { LeaveLobby(); });
        instaKill.onClick.AddListener(delegate { SetInstaKill(); });
    }

    private void ResetLobby()
    {
        foreach (Transform item in playerPanel.gameObject.transform)
        {
            Destroy(item.gameObject);
        }
        lobbyName.text = "Loading lobby...";
        lobbyCode.text = "";
        startGame.gameObject.SetActive(false);
    }

    private void UpdateLobby(Lobby lobby)
    {
        startGame.gameObject.SetActive(LobbyHandler.host);
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
        instaKill.image.color = LobbyHandler.isInstaKill ? Color.green : Color.red;
    }

    private void StartGame()
    {
        LobbyHandler.StartGame();
    }

    private void LeaveLobby()
    {
        LobbyHandler.LeaveLobby();
    }

    private void SetInstaKill()
    {
        LobbyHandler.SetInstaKill(!LobbyHandler.isInstaKill);
    }
}
