using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using static Constants;
using System.Linq;
using Menu2.Play;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using WebSocketSharp;

/*
 * Handles communication with the LobbyService
 */
public struct LobbyInfo
{
    public int playerCount;
    
    public Dictionary<int,bool> modifiers;
}
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

    public static Lobby currentLobby => instance._currentLobby;
    public static bool isHost => instance._isHost;
    public static bool isReady => instance._isReady;

    private Lobby _currentLobby;
    private bool _isHost;
    private bool _isReady;
    
    private LobbyPlayer _localPlayer;

    private float _heartbeatTimer;
    private float _refreshTimer;

    private static LobbyManager instance;

    public static LobbyInfo lobbyInfo;

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
                if (_heartbeatTimer + LOBBY_HEARTBEAT_COOLDOWN < Time.time)
                {
                    _heartbeatTimer = Time.time;

                    await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                }
            }

            if (_refreshTimer + LOBBY_UPDATE_COOLDOWN < Time.time)
            {
                _refreshTimer = Time.time;

                
                //Checking if players have been modified
                string oldModifiers = _currentLobby.Data[KEY_LOBBY_MODIFIER].Value;
                
                HashSet<string> oldPlayers = new HashSet<string>(_currentLobby.Players.Select(p => p.Id + p.Data[KEY_PLAYER_READY].Value));

                _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);

                HashSet<string> newPlayers = new HashSet<string>(_currentLobby.Players.Select(p => p.Id + p.Data[KEY_PLAYER_READY].Value));

                if(!oldPlayers.SetEquals(newPlayers))
                    onPlayerUpdate?.Invoke();
                
                //Checking if the lobby has been modified
                string newModifiers = _currentLobby.Data[KEY_LOBBY_MODIFIER].Value;
                
                if(!oldModifiers.Equals(newModifiers))
                    onLobbyUpdate?.Invoke();

                if (_currentLobby.Data[KEY_LOBBY_RELAYCODE].Value != "0")
                {
                    Debug.Log("Trying Relay Code");
                    bool success = await RelayManager.JoinGame(_currentLobby.Data[KEY_LOBBY_RELAYCODE].Value);

                    if (success)
                    {
                        SceneManager.LoadScene("Spoons");
                    }
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            LobbyFail();
        }
    }

    public async static Task<bool> CreateLobby()
    {
        if (instance._currentLobby != null)
            return false;
        try
        {
            string playerName = AuthenticationService.Instance.PlayerName ?? "SpoonsPlayer";
            var player = GetPlayer(playerName);
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = player,
                Data = new Dictionary<string, DataObject>
            {
                { KEY_LOBBY_RELAYCODE , new DataObject(DataObject.VisibilityOptions.Member,"0") },
                { KEY_LOBBY_MODIFIER , new DataObject(DataObject.VisibilityOptions.Public,"{}") }
            }
            };
            instance._currentLobby = await LobbyService.Instance.CreateLobbyAsync(playerName+"'s Lobby", LOBBY_MAX_PLAYERS, options);
            instance._isHost = true;
            instance._localPlayer = player;
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
            var player = GetPlayer(playerName);
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
            {
                Player = player
            };
            instance._currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
            instance._localPlayer = player;
            onEnterLobby?.Invoke();
            return true;
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        return false;
    }

    public static async Task ReadyToggle()
    {
        try
        {
            var player = instance._localPlayer;
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
            instance._localPlayer = null;
            onExitLobby?.Invoke();
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public static async void StartGame()
    {
        if(!isHost)
            return;
        
        string code = await RelayManager.CreateGame(currentLobby.Players.Count);
        
        if(code.IsNullOrEmpty())
            return;
        
        Debug.Log("Code is valid with "+code);

        instance._currentLobby.Data[KEY_LOBBY_RELAYCODE] = new DataObject(DataObject.VisibilityOptions.Member,code);
        UpdateLobbyOptions options = new UpdateLobbyOptions()
        {
            IsLocked = true,
            Data = instance._currentLobby.Data
        };
        instance.LoadLobbyInfo();
        
        await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, options);
    }

    public static async Task<bool> SetModifier(int index, bool state)
    {
        try
        {
            Dictionary<int, bool> modifiers =
                JsonConvert.DeserializeObject<Dictionary<int, bool>>(instance._currentLobby.Data[KEY_LOBBY_MODIFIER]
                    .Value);
            
            modifiers[index] = state;
            
            string serializedOut = JsonConvert.SerializeObject(modifiers);
            
            instance._currentLobby.Data[KEY_LOBBY_MODIFIER] = new DataObject(DataObject.VisibilityOptions.Public,serializedOut);
            
            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = instance._currentLobby.Data
            };
            
            Debug.Log(serializedOut);
            
            await LobbyService.Instance.UpdateLobbyAsync(instance._currentLobby.Id, options);
            
            onLobbyUpdate?.Invoke();
            
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }

    private void LobbyFail()
    {
        onExitLobby?.Invoke();
        _localPlayer = null;
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

    private void LoadLobbyInfo()
    {
        Dictionary<int, bool> incomingModifiers =
            JsonConvert.DeserializeObject<Dictionary<int, bool>>(instance._currentLobby.Data[KEY_LOBBY_MODIFIER].Value);

        lobbyInfo = new LobbyInfo()
        {
            playerCount = instance._currentLobby.Players.Count,
            modifiers = incomingModifiers
        };
    }
}
