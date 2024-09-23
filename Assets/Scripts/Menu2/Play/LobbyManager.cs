using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using static Constants;

/*
 * Handles communication with the LobbyService
 */
public class LobbyManager : MonoBehaviour
{
    public static Lobby currentLobby { get { return instance._currentLobby; } }

    private Lobby _currentLobby;
    private bool isHost;

    private float hearbeatTimer;
    private float refreshTimer;

    private static LobbyManager instance;

    void Start()
    {
        instance = this;
    }

    private async void Update()
    {
        if (_currentLobby == null)
            return;

        if (isHost)
        {
            if (hearbeatTimer + LOBBY_HEARTBEAT_COOLDOWN > Time.time)
                return;

            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);

            hearbeatTimer = Time.time;
        }

        if (refreshTimer + LOBBY_UPDATE_COOLDOWN > Time.time)
            return;

        _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
        refreshTimer = Time.time;
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
                Player = instance.GetPlayer(playerName),
                Data = new Dictionary<string, DataObject>
            {
                { KEY_LOBBY_RELAYCODE , new DataObject(DataObject.VisibilityOptions.Member,"0") },
                { KEY_LOBBY_MODIFIER_INSTAKILL , new DataObject(DataObject.VisibilityOptions.Public,"0") }
            }
            };
            await LobbyService.Instance.CreateLobbyAsync(playerName, 8, options);
            return true;
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        return false;
    }

    private LobbyPlayer GetPlayer(string name)
    {
        return new LobbyPlayer()
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,name) }
            }
        };
    }
}
