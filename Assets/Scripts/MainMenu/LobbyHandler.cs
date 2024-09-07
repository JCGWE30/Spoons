using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using static Constants;
using Unity.Mathematics;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

public class LobbyHandler : MonoBehaviour
{
    private static LobbyHandler instance;

    public delegate void OnLobbyUpdate();
    public delegate void OnEnterLobby(Lobby lobby);

    public static OnEnterLobby onUpdate;
    public static OnEnterLobby onEnterLobby;
    public static OnLobbyUpdate onExitLobby;

    private bool isHost;
    private Lobby lobby;
    private LobbyPlayer player;

    private float heartbeatTimer = 0f;
    private float fetchTimer = 0f;

    private bool isStarting = false;

    private string localName;

    void Start()
    {
        instance = this;
    }

    #region lobbyHeartbeat
    private async void Update()
    {
        HandleHeartbeat();
        await HandleFetchHeartbeat();
    }
    private async void HandleHeartbeat()
    {
        if (lobby == null || !isHost)
            return;
        if (Time.time > heartbeatTimer)
        {
            heartbeatTimer = LOBBY_HEARTBEAT_COOLDOWN + Time.time;
            if(!isStarting)
                await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
        }
    }
    private async Task HandleFetchHeartbeat()
    {
        if (lobby == null)
            return;
        if (Time.time > fetchTimer)
        {
            fetchTimer = LOBBY_UPDATE_COOLDOWN + Time.time;
            lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
            onUpdate?.Invoke(lobby);
            if (lobby.Data[KEY_RELAY_CODE].Value != "0")
            {
                JoinGame(lobby.Data[KEY_RELAY_CODE].Value);
            }
        }
    }
    #endregion

    public static void StartLobby(string name)
    {
        instance.HandleStartLobby(name);
    }
    public async static Task<bool> JoinLobby(string code,string name)
    {
        return await instance.HandleJoinLobby(code,name);
    }
    public static void LeaveLobby()
    {
        instance.HandleLeaveLobby();
    }
    public static void StartGame()
    {
        instance.HandleStartGame();
    }
    private async void HandleStartGame()
    {
        if (!isHost)
            return;
        if (isStarting)
            return;
        isStarting = true;
        string code = await RelayManager.CreateGame(lobby.Players.Count, localName);
        Debug.Log("Starting game with relay code " + code+ " "+KEY_RELAY_CODE);
        await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions()
        {
            IsLocked = true,
            Data = new Dictionary<string, DataObject>
            {
                { KEY_RELAY_CODE , new DataObject(DataObject.VisibilityOptions.Member,code) }
            }
        });
    }
    private async void HandleLeaveLobby()
    {
        await LobbyService.Instance.RemovePlayerAsync(lobby.Id,AuthenticationService.Instance.PlayerId);
        onExitLobby?.Invoke();
    }
    private async Task<bool> HandleJoinLobby(string code,string name)
    {
        UpdatePlayer(name);
        Lobby localLobby;
        if (code == null)
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions()
            {
                Player = player
            };
            localLobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
        }
        else
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions()
            {
                Player = player
            };
            localLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code,options);
        }
        if (localLobby == null)
            return false;
        lobby = localLobby;
        onEnterLobby?.Invoke(lobby);
        return true;
    }
    private async void HandleStartLobby(string name)
    {
        isHost = true;
        UpdatePlayer(name);
        CreateLobbyOptions options = new CreateLobbyOptions()
        {
            IsPrivate = false,
            Player = player,
            Data = new Dictionary<string, DataObject>
            {
                { KEY_RELAY_CODE , new DataObject(DataObject.VisibilityOptions.Member,"0") }
            }
        };

        Debug.Log("Shits GOIN");

        lobby = await LobbyService.Instance.CreateLobbyAsync(name + "'s lobby", LOBBY_MAX_PLAYERS,options);
        onEnterLobby?.Invoke(lobby);
        Debug.Log("Lobby created");
    }
    private void UpdatePlayer(string name)
    {
        localName = name;
        player = new LobbyPlayer()
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,name) }
            }
        };
    }

    private async void JoinGame(string relayCode)
    {
        if(!isHost)
            await RelayManager.JoinRelay(relayCode, localName);
        SceneManager.LoadScene("Spoons");
    }
}
