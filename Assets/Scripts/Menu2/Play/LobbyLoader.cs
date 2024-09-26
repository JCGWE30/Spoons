using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
 * Handles initalizing and updating player states
 * while in the lobby
 */
public class LobbyLoader : MonoBehaviour
{
    [SerializeField] private GameObject mainpanel;
    [SerializeField] private GameObject playerHolder;
    [SerializeField] private GameObject playerNameHolder;
    [SerializeField] private TMP_Text lobbyName;

    private static LobbyLoader instance;

    private void Awake()
    {
        instance = this;
        LobbyManager.onPlayerUpdate += UpdateLobby;
        LobbyManager.onExitLobby += ExitLobby;

        MenuTransition.onTransitionStart += (d) => { mainpanel.SetActive(false); };
        MenuTransition.onTransitionComplete += (d) => { mainpanel.SetActive(d == TransitionMenu.LobbyMenu); };
    }

    private void ExitLobby()
    {   
        MenuTransition.StartMove(TransitionMenu.PlayMenu, 1f);
    }

    public static void UpdateLobby()
    {
        var lobby = LobbyManager.currentLobby;
        instance.lobbyName.text = lobby.Name;

        GameObject nameHolder = instance.playerNameHolder;

        var allMarkers = instance.playerHolder.GetComponentsInChildren<PlayerIDHolder>(true);

        List<PlayerIDHolder> bindedMarkers = allMarkers.Where(m=>m.TryGet(out var _)).ToList();
        Stack<PlayerIDHolder> unbindedMarkers = new Stack<PlayerIDHolder>(allMarkers.Where(m => !m.TryGet(out var _)));

        Dictionary<string,LobbyPlayer> existingPlayerIds = lobby.Players.ToDictionary(p => p.Id, p => p);

        Stack<LobbyPlayer> players = new Stack<LobbyPlayer>(lobby.Players);

        foreach(PlayerIDHolder playerId in bindedMarkers)
        {
            if (existingPlayerIds.Keys.Contains(playerId.Get().Id)){
                playerId.Set(existingPlayerIds[playerId.Get().Id]);
                continue;
            }
            playerId.Wipe();
            unbindedMarkers.Push(playerId);
        }

        while(players.Count>0)
        {
            LobbyPlayer currentPlayer = players.Pop();
            if(PlayerIDHolder.TryGetPlayer(currentPlayer.Id, out var _))
            {
                continue;
            }

            PlayerIDHolder newHolder = unbindedMarkers.Pop();
            newHolder.Set(currentPlayer);
        }
        int index = 0;

        foreach (HoverButton button in instance.playerNameHolder.GetComponentsInChildren<HoverButton>())
        {
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if(lobby.Players.Count > index)
            {
                var player = lobby.Players[index];
                Debug.Log("Cycling on " + player);

                if(PlayerIDHolder.TryGetPlayer(player.Id,out var currentPlayer))
                {
                    button.onHover = () => currentPlayer.SetHighlightState(true);
                    button.onUnHover = () => currentPlayer.SetHighlightState(false);
                }
                else
                {
                    button.Wipe();
                }

                var info = LobbyManager.GetData(player);
                text.text = info.name;
            }
            else
            {
                text.text = string.Empty;
            }
            index++;
        }
    }
}
