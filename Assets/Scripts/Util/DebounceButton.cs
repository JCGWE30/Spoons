using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebounceButton : MonoBehaviour
{
    public delegate void ButtonHandler();

    public ButtonHandler onClick;

    public float debounce;
    private float debounceTimer;

    private void Start()
    {
        if(!TryGetComponent<Button>(out var button))
        {
            Destroy(this);
            return;
        }

        button.onClick.AddListener(ClickProcess);
    }

    private void ClickProcess()
    {
        if (debounceTimer + debounce > Time.time)
            return;
        debounceTimer = Time.time;
        onClick?.Invoke();
    }
}
