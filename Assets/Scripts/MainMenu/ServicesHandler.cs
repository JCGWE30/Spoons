using ParrelSync;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class ServicesHandler : MonoBehaviour
{
    public delegate void ServicesStarthandler();
    public ServicesStarthandler onServiceStart;
    private async void Start()
    {
        if (UnityServices.State==ServicesInitializationState.Initialized)
        {
            onServiceStart?.Invoke();
            return;
        }
        var options = new InitializationOptions();
        options.SetProfile("Normal");
#if UNITY_EDITOR
        if (ClonesManager.IsClone())
            options.SetProfile(ClonesManager.GetArgument());
#endif
        await UnityServices.InitializeAsync(options);
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        onServiceStart?.Invoke();
    }
}
