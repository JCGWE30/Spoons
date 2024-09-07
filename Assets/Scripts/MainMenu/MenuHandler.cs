using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private Image lobbyPanel;
    [SerializeField] private TMP_Text outputMessage;
    [SerializeField] private Image mainPanel;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button rulesButton;

    [SerializeField] private Button codeJoinButton;
    [SerializeField] private TMP_InputField codeJoinInput;
    [SerializeField] private TMP_InputField setNameInput;

    private string playerName;
    private static MenuHandler instance;

    void Start()
    {
        instance = this;
        Debug.Log(lobbyPanel+" Is the panel");
        lobbyPanel.gameObject.SetActive(false);
        mainPanel.gameObject.SetActive(true);

        hostButton.onClick.AddListener(delegate { StartLobby(); });
        quickJoinButton.onClick.AddListener(delegate { QuickJoin(); });
        codeJoinButton.onClick.AddListener(delegate { JoinWithCode(); });
        rulesButton.onClick.AddListener(GoRules);

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

    private void GoRules()
    {
        instance.mainPanel.gameObject.SetActive(false);
        RulesHandler.EnterRules();
    }

    public static void FromRules()
    {
        instance.mainPanel.gameObject.SetActive(true);
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
