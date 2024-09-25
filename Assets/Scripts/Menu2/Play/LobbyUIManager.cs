using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private DebounceButton startButton;
    [SerializeField] private DebounceButton readyButton;
    [SerializeField] private DebounceButton leaveButton;

    void Start()
    {
        startButton.debounce = Constants.LOBBY_BUTTON_COOLDOWN;
        readyButton.debounce = Constants.LOBBY_BUTTON_COOLDOWN;
        leaveButton.debounce = Constants.LOBBY_BUTTON_COOLDOWN;

        LobbyManager.onEnterLobby += SetupLobby;
        LobbyManager.onLobbyUpdate += UpdateStatus;

    }

    private void SetupLobby()
    {
        startButton.gameObject.SetActive(LobbyManager.isHost);
        UpdateStatus();
    }

    private void UpdateStatus()
    {

    }
}
