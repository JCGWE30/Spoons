
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Lobbies;
using UnityEngine;

public class ServicesHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text playerId;

    public delegate void ServicesStarthandler();
    public static ServicesStarthandler onServiceStart;
    private async void Start()
    {
        LobbyHandler.onEnterGame += () => onServiceStart = null;

        if (UnityServices.State==ServicesInitializationState.Initialized)
        {
            onServiceStart?.Invoke();
            playerId.text = "PlayerID: " + AuthenticationService.Instance.PlayerId;
            return;
        }
        var options = new InitializationOptions();
        options.SetProfile("Normal");
        options.SetEnvironmentName("production");
#if UNITY_EDITOR
        options.SetEnvironmentName("dev");
        if (ParrelSync.ClonesManager.IsClone())
            options.SetProfile(ParrelSync.ClonesManager.GetArgument());
#endif
        await UnityServices.InitializeAsync(options);
        AuthenticationService.Instance.SignedIn += async () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            if (AuthenticationService.Instance.PlayerName == null)
                await AuthenticationService.Instance.UpdatePlayerNameAsync(Constants.AUTH_DEFAULTNAME);
            playerId.text = "PlayerID: " + AuthenticationService.Instance.PlayerId;
            onServiceStart?.Invoke();
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public static async Task<bool> UpdateName(string newName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            return true;
        } catch (AuthenticationException _)
        {
            return false;
        }
    }
}
