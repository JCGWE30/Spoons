using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private InputField username;
    [SerializeField] private InputField password;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private TMP_Text title;

    private bool registering = false;

    // Start is called before the first frame update
    private async void Start()
    {
        InitializationOptions options = new InitializationOptions();
#if UNITY_EDITOR
        options.SetEnvironmentName("dev");
#endif
        await UnityServices.InitializeAsync(options);
        loginButton.onClick.AddListener(AttemptLogin);
        registerButton.onClick.AddListener(SwitchRegister);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwitchRegister()
    {
        registering = !registering;

        registerButton.GetComponentInChildren<TMP_Text>().text = registering ? "Switch to login" : "Switch to register";
        loginButton.GetComponentInChildren<TMP_Text>().text = registering ? "Register" : "Login";
        title.GetComponentInChildren<TMP_Text>().text = registering ? "REGISTER" : "LOGIN";
    }

    private async void AttemptLogin()
    {
        if (username == null)
        {
            //TODO show error
            return;
        }

        if (password == null)
        {
            //TODO show error
            return;
        }

        try
        {
            if (registering)
            {
                if (CheckLoginParams())
                    await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username.text, password.text);
            }
            else
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username.text, password.text);
            }
        }catch(AuthenticationException e)
        {
            Debug.Log(e.StackTrace);
        }
    }

    private bool CheckLoginParams()
    {
        Regex usernameRegex = new Regex("^[a-zA-Z0-9.\\-@_]$");
        Regex passwordRegex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\w\\s])[^\\s]{8,30}$");

        if (!usernameRegex.IsMatch(username.text))
        {
            //TODO print username error
            return false;
        }

        if (!passwordRegex.IsMatch(password.text))
        {
            //TODO print password error
            return false;
        }

        return true;
    }
}
