using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public enum ModifierEnum
{
    instakill = 0,
    infinideck = 1,
    onespoon = 2,
    fullvision = 3,
    timed = 4
}
public class ModifierButton : MonoBehaviour
{
    private bool _modifierState;
    private DebounceButton button;
    
    [SerializeField] private ModifierEnum modifier;
    //Handles buttons that act as modifiers
    void Start()
    {
        button = GetComponent<DebounceButton>();

        button.debounce = Constants.LOBBY_BUTTON_COOLDOWN;
        button.onClick = HandleClick;

        LobbyManager.onLobbyUpdate = UpdateLobby;
    }
    
    //On click send event to update modifier
    
    //Handle lobby Update

    private void UpdateLobby()
    {
        Dictionary<int, bool> modifiers =
            JsonConvert.DeserializeObject<Dictionary<int, bool>>(LobbyManager.currentLobby.Data[Constants.KEY_LOBBY_MODIFIER].Value);

        _modifierState = modifiers.GetValueOrDefault((int)modifier,false);
        Debug.Log("Update detected, setting modifier state to "+_modifierState);
        button.GetComponent<Image>().color = _modifierState ? Color.green : Color.red;
    }

    private async void HandleClick()
    {
        foreach (ModifierButton modButton in FindObjectsOfType<ModifierButton>())
        {
            modButton.GetComponent<DebounceButton>().Trigger();
        }
        
        bool success = await LobbyManager.SetModifier((int) modifier,!_modifierState);

        if (success)
        {
            _modifierState = !_modifierState;
        }
    }
}
