using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


/*
 * Handles the lobby Browser
 */
public class LobbyInitalizer : MonoBehaviour
{
    [SerializeField] private GameObject mainMarker;

    [SerializeField] private DebounceButton createLobby;
    [SerializeField] private DebounceButton refreshLobbies;

    [SerializeField] private Button backButton;
    [SerializeField] private RectTransform lobbyPanel;

    [SerializeField] private LobbyObjectManager lobbyPrefab;

    [SerializeField] private Transform menuMarker;

    private void Awake()
    {
        createLobby.debounce = Constants.RATELIMIT_LOBBY_CREATE;
        refreshLobbies.debounce = Constants.RATELIMIT_LOBBY_QUERY;

        createLobby.onClick = CreateLobby;

        backButton.onClick.AddListener(Back);

        MenuTransition.onTransitionStart += (TransitionMenu menu) =>
        {
            Debug.Log("Transition starting to " + menu);
            if (menu != TransitionMenu.PlayMenu) return; 
            UpdateLobbies();
        };
    }

    private void Back()
    {
        MenuTransition.StartMove(TransitionMenu.MainMenu, 1f);
    }

    private async void CreateLobby()
    {
        bool success = await LobbyManager.CreateLobby();

        if (success)
        {
            MenuTransition.StartMove(TransitionMenu.LobbyMenu, 1f);
            LobbyLoader.UpdateLobby();
        }
    }

    private async void UpdateLobbies()
    {
        foreach(Transform transform in lobbyPanel)
        {
            Destroy(transform.gameObject);
        }

        QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync();
        Debug.Log("Lobbies Found " + lobbies.Results.Count);

        foreach(Lobby lobby in lobbies.Results)
        {
            LobbyObjectManager lobbyInfo = Instantiate(lobbyPrefab);
            lobbyInfo.SetLobby(lobby);
            lobbyInfo.transform.SetParent(lobbyPanel,false);
        }
    }
}
