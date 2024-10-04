using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebounceButton : MonoBehaviour
{
    public delegate void ButtonHandler();

    public ButtonHandler onClick;

    public string text
    {
        set => _button.GetComponentInChildren<TMP_Text>().text = value;
    }
    
    public bool buttonEnabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            GetComponent<Image>().color = value ? Color.white : Color.black;
        }
    }

    public float debounce;

    private bool _enabled = true;
    private Button _button;
    private float _debounceTimer;

    private void Start()
    {
        if(!TryGetComponent(out _button))
        {
            Destroy(this);
            return;
        }
        
        _button.onClick.AddListener(ClickProcess);
    }

    private void ClickProcess()
    {
        if (!_enabled)
            return;
        if (_debounceTimer + debounce > Time.time)
            return;
        _debounceTimer = Time.time;
        onClick?.Invoke();
    }

    public void Trigger()
    {
        _debounceTimer = Time.time;
    }
}
