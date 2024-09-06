using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;

public class ServicesHandler : MonoBehaviour
{
    public delegate void ServicesStarthandler();
    public ServicesStarthandler onServiceStart;
    void Start()
    {
        if (UnityServices.State==ServicesInitializationState.Initialized)
        {
            onServiceStart?.Invoke();
            return;
        }
        UnityServices.InitializeAsync();
        onServiceStart?.Invoke();
    }
}
