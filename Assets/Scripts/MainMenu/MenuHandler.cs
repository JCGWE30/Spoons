using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MenuObjectHolder;

public class MenuHandler : MonoBehaviour
{
    private string playerName;

    void Start()
    {
        Debug.Log(lobbyPanel+" Is the panel");
        lobbyPanel.gameObject.SetActive(false);
        mainPanel.gameObject.SetActive(true);

        hostButton.onClick.AddListener(delegate { StartLobby(); });
        quickJoinButton.onClick.AddListener(delegate { QuickJoin(); });
        codeJoinButton.onClick.AddListener(delegate { JoinWithCode(); });

        LobbyHandler.onEnterLobby += EnterLobby;
        LobbyHandler.onExitLobby += ExitLobby;
    }

    private bool CanLobby()
    {
        playerName = setNameInput.text;
        if (playerName.Length<1)
        {
            OutputText(Constants.TEXTS_NONAME, false);
            return false;
        }
        return true;
    }

    private void StartLobby()
    {
        if (!CanLobby())
            return;
        Debug.Log("Shits GOIN PT1");
        LobbyHandler.StartLobby(playerName);
    }

    private async void QuickJoin()
    {
        if (!CanLobby())
            return;
        if (!await LobbyHandler.JoinLobby(null,playerName))
            OutputText(Constants.TEXTS_NOLOBBY, false);
    }

    private async void JoinWithCode()
    {
        if (!CanLobby())
            return;

        string code = codeJoinInput.text;
        if (code == null)
        {
            OutputText(Constants.TEXTS_NOCODE, false);
            return;
        }

        if (!await LobbyHandler.JoinLobby(code,playerName))
            OutputText(Constants.TEXTS_NOLOBBY, false);
    }

    private void EnterLobby(Lobby lobby)
    {
        mainPanel.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(true);
    }

    private void ExitLobby()
    {
        mainPanel.gameObject.SetActive(true);
        lobbyPanel.gameObject.SetActive(false);
    }


    private void OutputText(string text, bool good)
    {
        outputMessage.text = text;
        outputMessage.color = good ? Color.green : Color.red;
    }
}
