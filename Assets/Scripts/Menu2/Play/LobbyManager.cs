using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using static Constants;
using System.Linq;

/*
 * Handles communication with the LobbyService
 */
public struct LobbyPlayerInfo
{
    public string name;
    public string skin;
    public bool ready;
}
public class LobbyManager : MonoBehaviour
{
    public delegate void LobbyChangeEvent();

    public static LobbyChangeEvent onPlayerUpdate;
    public static LobbyChangeEvent onLobbyUpdate;
    public static LobbyChangeEvent onEnterLobby;
    public static LobbyChangeEvent onExitLobby;

    public static Lobby currentLobby { get { return instance._currentLobby; } }
    public static bool isHost { get { return instance._isHost; } }

    private Lobby _currentLobby;
    private bool _isHost;
    private bool _isReady;

    private float hearbeatTimer;
    private float refreshTimer;

    private static LobbyManager instance;

    void Awake()
    {
        onPlayerUpdate = null;
        instance = this;
    }

    private async void Update()
    {
        try
        {
            if (_currentLobby == null)
                return;

            if (isHost)
            {
                if (hearbeatTimer + LOBBY_HEARTBEAT_COOLDOWN > Time.time)
                    return;

                hearbeatTimer = Time.time;

                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            }

            if (refreshTimer + LOBBY_UPDATE_COOLDOWN < Time.time)
            {
                refreshTimer = Time.time;

                HashSet<string> oldPlayers = new HashSet<string>(_currentLobby.Players.Select(p => p.Id + p.Data[KEY_PLAYER_READY]));

                _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);

                HashSet<string> newPlayers = new HashSet<string>(_currentLobby.Players.Select(p => p.Id + p.Data[KEY_PLAYER_READY]));

                if(!oldPlayers.SetEquals(newPlayers))
                    onPlayerUpdate?.Invoke();
            }
        }
        catch (LobbyServiceException e)
        {
            LobbyFail();
            Debug.Log(e);
        }
    }

    public async static Task<bool> CreateLobby()
    {
        if (instance._currentLobby != null)
            return false;
        try
        {
            string playerName = AuthenticationService.Instance.PlayerName ?? "SpoonsPlayer";
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = GetPlayer(playerName),
                Data = new Dictionary<string, DataObject>
            {
                { KEY_LOBBY_RELAYCODE , new DataObject(DataObject.VisibilityOptions.Member,"0") },
                { KEY_LOBBY_MODIFIER_INSTAKILL , new DataObject(DataObject.VisibilityOptions.Public,"0") }
            }
            };
            instance._currentLobby = await LobbyService.Instance.CreateLobbyAsync(playerName+"'s Lobby", LOBBY_MAX_PLAYERS, options);
            instance._isHost = true;
            onEnterLobby?.Invoke();
            return true;
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        return false;
    }

    public async static Task<bool> JoinLobby(Lobby lobby)
    {
        if(instance._currentLobby != null) return false;

        try
        {
            string playerName = AuthenticationService.Instance.PlayerName ?? "SpoonsPlayer " + lobby.Players.Count;
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer(playerName)
            };
            instance._currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
            onEnterLobby?.Invoke();
            return true;
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        return false;
    }

    public async static void ReadyToggle()
    {
        try
        {
            string name = AuthenticationService.Instance.PlayerName ?? "SpoonsPlayer";

            LobbyPlayer player = GetPlayer(name);
            player.Data[KEY_PLAYER_READY] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, !instance._isReady ? "1" : "0");

            UpdatePlayerOptions options = new UpdatePlayerOptions()
            {
                Data = player.Data
            };

            await LobbyService.Instance.UpdatePlayerAsync(instance._currentLobby.Id, AuthenticationService.Instance.PlayerId, options);
            instance._isReady = !instance._isReady;
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public static async void LeaveLobby()
    {
        try
        {
            if (instance._currentLobby == null)
                return;

            if (instance._isHost)
            {
                await LobbyService.Instance.DeleteLobbyAsync(instance._currentLobby.Id);
            }
            else
            {
                await LobbyService.Instance.RemovePlayerAsync(instance._currentLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            instance._isHost = false;
            instance._currentLobby = null;
            onExitLobby?.Invoke();
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void LobbyFail()
    {
        onExitLobby?.Invoke();
        _isHost = false;
        _currentLobby = null;
    }

    public static async Task<List<Lobby>> GetLobbies()
    {
        QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
        return response.Results;
    }

    private static LobbyPlayer GetPlayer(string name)
    {
        return new LobbyPlayer()
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,name) },
                { KEY_PLAYER_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,instance._isReady ? "1" : "0") }
            }
        };
    }

    public static LobbyPlayerInfo GetData(LobbyPlayer player)
    {
        return new LobbyPlayerInfo()
        {
            name = player.Data[KEY_PLAYER_NAME].Value,
            ready = player.Data[KEY_PLAYER_READY].Value == "1"
        };
    }

    public static LobbyPlayer GetPlayer(string id, Lobby lobby)
    {
        foreach(LobbyPlayer player in lobby.Players)
        {
            if (player.Id == id)
            {
                return player;
            }
        }
        return null;
    }
}
