using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInitalizer : MonoBehaviour
{
    [SerializeField] private GameObject mainMarker;

    [SerializeField] private DebounceButton createLobby;
    [SerializeField] private DebounceButton refreshLobbies;

    [SerializeField] private Button backButton;
    [SerializeField] private RectTransform lobbyPanel;

    [SerializeField] private GameObject lobbyPrefab;

    [SerializeField] private Transform menuMarker;

    private void Start()
    {
        createLobby.debounce = Constants.RATELIMIT_LOBBY_CREATE;
        refreshLobbies.debounce = Constants.RATELIMIT_LOBBY_QUERY;

        createLobby.onClick = CreateLobby;

        backButton.onClick.AddListener(Back);
    }

    private void Back()
    {
        MenuTransition.StartMove(TransitionMenu.MainMenu, 1f);
    }

    private async void CreateLobby()
    {
        bool success = LobbyManager.CreateLobby();
    }
}
