using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{
    [SerializeField] TMP_Text outputMessage;
    [SerializeField] Image mainPanel;
    [SerializeField] Button hostButton;
    [SerializeField] Button quickJoinButton;

    [SerializeField] Button codeJoinButton;
    [SerializeField] Button setNameButton;
    [SerializeField] TMP_InputField codeJoinInput;
    [SerializeField] TMP_InputField setNameInput;
    void Start()
    {
        hostButton.onClick.AddListener( delegate { } );
        quickJoinButton.onClick.AddListener( delegate { } );
        codeJoinButton.onClick.AddListener( delegate { } );
        setNameButton.onClick.AddListener( delegate { } );
    }
}
