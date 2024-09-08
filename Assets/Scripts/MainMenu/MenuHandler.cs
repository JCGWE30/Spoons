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
    [SerializeField] private Image mainPanel;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button settingsButton;

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
        settingsButton.onClick.AddListener(GoSettings);

        LobbyHandler.onEnterLobby += EnterLobby;
        LobbyHandler.onExitLobby += ExitLobby;
    }

    private bool CanLobby()
    {
        playerName = setNameInput.text;
        if (playerName.Length<1)
        {
            ErrorReporter.Throw(Constants.TEXTS_NONAME);
            return false;
        }
        if (playerName.Length > 16)
        {
            ErrorReporter.Throw(Constants.TEXTS_LONGNAME);
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
            ErrorReporter.Throw(Constants.TEXTS_NOLOBBY);
    }

    private async void JoinWithCode()
    {
        if (!CanLobby())
            return;

        string code = codeJoinInput.text;
        if (code == null)
        {
            ErrorReporter.Throw(Constants.TEXTS_NOCODE);
            return;
        }

        if (!await LobbyHandler.JoinLobby(code,playerName))
            ErrorReporter.Throw(Constants.TEXTS_NOLOBBY);
    }

    private void GoRules()
    {
        instance.mainPanel.gameObject.SetActive(false);
        RulesHandler.EnterRules();
    }

    private void GoSettings()
    {
        instance.mainPanel.gameObject.SetActive(false);
        SettingsHandler.EnterSettings();
    }

    public static void BackToMenu()
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
}
