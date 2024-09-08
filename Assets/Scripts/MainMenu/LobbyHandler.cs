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
    public static OnLobbyUpdate onEnterGame;

    private bool isHost;
    public static bool isInstaKill;
    public static bool host { get { return instance.isHost; } }
    private Lobby lobby;
    private LobbyPlayer player;

    private float heartbeatTimer = 0f;
    private float fetchTimer = 0f;

    private bool isStarting = false;

    private string localName;

    void Start()
    {
        instance = this;
        isInstaKill = false;
    }

    #region lobbyHeartbeat
    private async void Update()
    {
        HandleHeartbeat();
        await HandleFetchHeartbeat();
    }
    private async void HandleHeartbeat()
    {
        try
        {
            if (lobby == null || !isHost)
                return;
            if (Time.time > heartbeatTimer)
            {
                heartbeatTimer = LOBBY_HEARTBEAT_COOLDOWN + Time.time;
                if (!isStarting)
                    await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
            }
        }
        catch (LobbyServiceException e)
        {
            HandleError(e);
        }
    }
    private async Task HandleFetchHeartbeat()
    {
        try
        {
            if (lobby == null)
                return;
            if (Time.time > fetchTimer)
            {
                fetchTimer = LOBBY_UPDATE_COOLDOWN + Time.time;
                lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
                isInstaKill = lobby.Data[KEY_INSTA_KILL].Value == "true";
                onUpdate?.Invoke(lobby);
                if (lobby.Data[KEY_RELAY_CODE].Value != "0")
                {
                    JoinGame(lobby.Data[KEY_RELAY_CODE].Value);
                }
            }
        }catch(LobbyServiceException e)
        {
            HandleError(e);
        }
    }
    #endregion

    public static void StartLobby(string name)
    {
        instance.HandleStartLobby(name);
    }
    public async static void JoinLobby(string code,string name)
    {
        await instance.HandleJoinLobby(code,name);
    }
    public static void LeaveLobby()
    {
        instance.HandleLeaveLobby();
    }
    public static void StartGame()
    {
        instance.HandleStartGame();
    }
    public static void SetInstaKill(bool state)
    {
        instance.HandleInstaKill(state);
    }
    private async void HandleStartGame()
    {
        try
        {
            if (!isHost)
                return;
            if (isStarting)
                return;
            isStarting = true;
            string code = await RelayManager.CreateGame(lobby.Players.Count, localName);
            Debug.Log("Starting game with relay code " + code + " " + KEY_RELAY_CODE);
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions()
            {
                IsLocked = true,
                IsPrivate = true,
                Data = new Dictionary<string, DataObject>
            {
                { KEY_RELAY_CODE , new DataObject(DataObject.VisibilityOptions.Member,code) }
            }
            });
        }catch(LobbyServiceException e)
        {
            HandleError(e);
        }
    }
    private async void HandleInstaKill(bool state)
    {
        try
        {
            if (isStarting)
                return;
            if (!isHost)
                return;
            string stateString = state ? "true" : "false";
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>
            {
                { KEY_INSTA_KILL , new DataObject(DataObject.VisibilityOptions.Public,stateString) }
            }
            });
        }catch(LobbyServiceException e)
        {
            HandleError(e);
        }
    }
    private async void HandleLeaveLobby()
    {
        try
        {
            if (isHost)
            {
                isHost = false;
                await LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
            }
            else
            {
                foreach (string lob in await LobbyService.Instance.GetJoinedLobbiesAsync())
                {
                    await LobbyService.Instance.RemovePlayerAsync(lob, AuthenticationService.Instance.PlayerId);
                }
            }
            lobby = null;
            onExitLobby?.Invoke();
        }catch(LobbyServiceException e)
        {
            HandleError(e);
        }
    }
    private async Task HandleJoinLobby(string code,string name)
    {
        try
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
                localLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            }
            if (localLobby == null)
                return;
            await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
            lobby = localLobby;
            onEnterLobby?.Invoke(lobby);
        }
        catch (LobbyServiceException e)
        {
            HandleError(e);
        }
    }
    private async void HandleStartLobby(string name)
    {
        try
        {
            isHost = true;
            UpdatePlayer(name);
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = player,
                Data = new Dictionary<string, DataObject>
            {
                { KEY_RELAY_CODE , new DataObject(DataObject.VisibilityOptions.Member,"0") },
                { KEY_INSTA_KILL , new DataObject(DataObject.VisibilityOptions.Public,"0") }
            }
            };
            fetchTimer = Time.time;
            heartbeatTimer = Time.time;
            lobby = await LobbyService.Instance.CreateLobbyAsync(name + "'s lobby", LOBBY_MAX_PLAYERS, options);
            onEnterLobby?.Invoke(lobby);
        }catch(LobbyServiceException e)
        {

            HandleError(e);
        }
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
        onEnterGame?.Invoke();
        onEnterGame = null;
        onUpdate = null;
        onEnterLobby = null;
        onExitLobby = null;
        SceneManager.LoadScene("Spoons");
    }

    private string GetErrorMessage(LobbyExceptionReason rs)
    {
        return rs switch
        {
            LobbyExceptionReason.LobbyNotFound => TEXTS_NOLOBBY,
            LobbyExceptionReason.LobbyConflict => TEXTS_CONFLICT,
            LobbyExceptionReason.LobbyFull => TEXTS_FULL,
            LobbyExceptionReason.NoOpenLobbies => TEXTS_NOLOBBIES,
            LobbyExceptionReason.LobbyAlreadyExists => TEXTS_CONFLICT,
            LobbyExceptionReason.InvalidJoinCode => TEXTS_NOCODE,
            _ => TEXTS_UNKNOWN
        };
    }

    private void HandleError(LobbyServiceException ex)
    {
        Debug.Log(ex.Reason);
        int fatal = ex.Reason switch
        {
            LobbyExceptionReason.LobbyNotFound => 1,
            LobbyExceptionReason.PlayerNotFound => 1,
            LobbyExceptionReason.LobbyConflict => 1,
            LobbyExceptionReason.LobbyFull => 1,
            LobbyExceptionReason.NoOpenLobbies => 1,
            LobbyExceptionReason.LobbyAlreadyExists => 1,
            LobbyExceptionReason.InvalidJoinCode => 1,
            LobbyExceptionReason.RateLimited => 0,
            _ => -1
        };

        if (fatal == 1)
        {
            ErrorReporter.Throw(GetErrorMessage(ex.Reason));
            if (lobby == null)
                return;
            lobby = null;
            onExitLobby?.Invoke();
        }
        if (fatal == -1)
            throw ex;
    }
}
