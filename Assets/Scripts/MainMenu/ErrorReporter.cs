using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorReporter : MonoBehaviour
{
    [SerializeField] private TMP_Text errorText;

    private static ErrorReporter instance;

    private float textStoredTime;
    private bool textStored;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (!textStored)
            return;
        if (Time.time > textStoredTime)
        {
            textStored = false;
            errorText.text = "";
        }
    }

    public static void Throw(string message)
    {
        instance.errorText.text = message;
        instance.errorText.color = Color.red;

        instance.textStored = true;
        instance.textStoredTime = Constants.PLAYER_TOPTEXT_TIME + Time.time;
    }

    public static void Success(string message)
    {
        instance.errorText.text = message;
        instance.errorText.color = Color.green;

        instance.textStored = true;
        instance.textStoredTime = Constants.PLAYER_TOPTEXT_TIME + Time.time;
    }
}
