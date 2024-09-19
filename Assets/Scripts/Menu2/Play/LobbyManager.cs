using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using static Constants;

public class LobbyManager : MonoBehaviour
{
    Lobby currentLobby;
    bool isHost;

    private static LobbyManager instance;

    void Start()
    {
        instance = this;
    }

    public async static Task<bool> CreateLobby()
    {
        try
        {
            string playerName = AuthenticationService.Instance.PlayerName;
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
