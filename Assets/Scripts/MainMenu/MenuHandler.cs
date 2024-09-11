using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
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
    [SerializeField] private InputField codeJoinInput;
    [SerializeField] private InputField setNameInput;

    [SerializeField] private Image leaderboard;

    private string playerName;
    private static MenuHandler instance;

    void Start()
    {
        instance = this;
        Debug.Log(lobbyPanel+" Is the panel");
        lobbyPanel.gameObject.SetActive(false);
        mainPanel.gameObject.SetActive(true);
        leaderboard.gameObject.SetActive(true);

        hostButton.onClick.AddListener(delegate { StartLobby(); });
        quickJoinButton.onClick.AddListener(delegate { QuickJoin(); });
        codeJoinButton.onClick.AddListener(delegate { JoinWithCode(); });
        rulesButton.onClick.AddListener(GoRules);
        settingsButton.onClick.AddListener(GoSettings);

        LobbyHandler.onEnterLobby += EnterLobby;
        LobbyHandler.onExitLobby += ExitLobby;
        ServicesHandler.onServiceStart += () => {
            var txt = AuthenticationService.Instance.PlayerName;
            setNameInput.text = txt.Split("#")[0];
        };
    }

    private async Task<bool> CanLobby()
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
        if (!playerName.All(char.IsLetterOrDigit))
        {
            ErrorReporter.Throw(Constants.TEXTS_ILLEGAL);
            return false;
        }
        return true;
    }

    private async void StartLobby()
    {
        if (!await CanLobby())
            return;
        Debug.Log("Shits GOIN PT1");
        LobbyHandler.StartLobby(playerName);
    }

    private async void QuickJoin()
    {
        if (!await CanLobby())
            return;
        LobbyHandler.JoinLobby(null, playerName);
    }

    private async void JoinWithCode()
    {
        if (!await CanLobby())
            return;

        string code = codeJoinInput.text;
        if (code == null)
        {
            ErrorReporter.Throw(Constants.TEXTS_NOCODE);
            return;
        }

        LobbyHandler.JoinLobby(code, playerName);
    }

    private void GoRules()
    {
        instance.mainPanel.gameObject.SetActive(false);
        leaderboard.gameObject.SetActive(false);
        RulesHandler.EnterRules();
    }

    private void GoSettings()
    {
        instance.mainPanel.gameObject.SetActive(false);
        leaderboard.gameObject.SetActive(false);
        SettingsHandler.EnterSettings();
    }

    public static void BackToMenu()
    {
        instance.mainPanel.gameObject.SetActive(true);
        instance.leaderboard.gameObject.SetActive(true);
    }

    private void EnterLobby(Lobby lobby)
    {
        mainPanel.gameObject.SetActive(false);
        leaderboard.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(true);
    }

    private void ExitLobby()
    {
        mainPanel.gameObject.SetActive(true);
        leaderboard.gameObject.SetActive(true);
        lobbyPanel.gameObject.SetActive(false);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
