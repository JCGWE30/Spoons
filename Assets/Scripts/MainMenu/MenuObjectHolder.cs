using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuObjectHolder : MonoBehaviour
{
    public static Image lobbyPanel { get; private set; }
    public static TMP_Text outputMessage { get; private set; }
    public static Image mainPanel { get; private set; }
    public static Button hostButton { get; private set; }
    public static Button quickJoinButton { get; private set; }

    public static Button codeJoinButton { get; private set; }
    public static TMP_InputField codeJoinInput { get; private set; }
    public static TMP_InputField setNameInput { get; private set; }


    public TMP_Text _playerPrefab;
    public static TMP_Text playerPrefab { get { return instance._playerPrefab; } }
    public static TMP_Text lobbyName { get; private set; }
    public static TMP_Text lobbyCode { get; private set; }
    public static Image playerPanel { get; private set; }

    public static Button startGame { get; private set; }
    public static Button leaveLobby { get; private set; }

    private static MenuObjectHolder instance;

    void Start()
    {
        instance = this;
        Debug.Log(gameObject.transform.Find("LobbyPanel"));
        lobbyPanel = gameObject.transform.Find("LobbyPanel").GetComponent<Image>();
        outputMessage = gameObject.transform.Find("OutputText").GetComponent<TMP_Text>();
        mainPanel = gameObject.transform.Find("MainPanel").GetComponent<Image>();
        hostButton = gameObject.transform.Find("MainPanel/HostButton").GetComponent<Button>();
        quickJoinButton = gameObject.transform.Find("MainPanel/QuickJoinButton").GetComponent<Button>();

        codeJoinButton = gameObject.transform.Find("MainPanel/JoinWithCode/CodeButton").GetComponent<Button>();
        codeJoinInput = gameObject.transform.Find("MainPanel/JoinWithCode/CodeField").GetComponent<TMP_InputField>();
        setNameInput = gameObject.transform.Find("MainPanel/Name/NameField").GetComponent<TMP_InputField>();

        lobbyName = gameObject.transform.Find("LobbyPanel/LobbyName").GetComponent<TMP_Text>();
        lobbyCode = gameObject.transform.Find("LobbyPanel/LobbyCode").GetComponent<TMP_Text>();
        playerPanel = gameObject.transform.Find("LobbyPanel/PlayerList").GetComponent<Image>();
        startGame = gameObject.transform.Find("LobbyPanel/Start").GetComponent<Button>();
        leaveLobby = gameObject.transform.Find("LobbyPanel/Leave").GetComponent<Button>();
    }
}
