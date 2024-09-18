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
    [SerializeField] private Image mainPanel;
    [SerializeField] private InputField username;
    [SerializeField] private InputField password;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private TMP_Text title;

    //DEBUG CREDS USER: hitheresafe PASS: HeyThere123!

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
        string usernameInput = username.text;
        string passwordInput = password.text;

        if (usernameInput == null)
        {
            //TODO show error
            return;
        }

        if (passwordInput == null)
        {
            //TODO show error
            return;
        }

        try
        {
            if (registering)
            {
                if (!CheckLoginParams())
                    return;
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(usernameInput, passwordInput);
            }
            else
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(usernameInput, passwordInput);
            }
            mainPanel.gameObject.SetActive(false);
            Debug.Log("Logged in as " + AuthenticationService.Instance.PlayerId);
        }catch(AuthenticationException e)
        {
            Debug.Log(e.StackTrace);
        }
    }

    private bool CheckLoginParams()
    {
        Regex usernameRegex = new Regex("^[a-zA-Z0-9.\\-@_]{3,20}$");
        Regex passwordRegex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\w\\s])[^\\s]{8,30}$");

        if (!usernameRegex.IsMatch(username.text))
        {
            Debug.Log("username mismatch");
            return false;
        }

        if (!passwordRegex.IsMatch(password.text))
        {
            Debug.Log("password mismatch");
            return false;
        }

        return true;
    }
}
