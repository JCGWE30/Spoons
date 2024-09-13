using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HideAfterTime : MonoBehaviour
{
    public delegate void EndDisplayHandler();


    private TMP_Text text;

    private float displayEnd;
    private bool running = false;
    private EndDisplayHandler onEnd;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public EndDisplayHandler Display(string message,float time)
    {
        text.text = message;
        displayEnd = Time.time + time;
        running = true;
        return onEnd;
    }

    public EndDisplayHandler Display(string message, float time, Color color)
    {
        text.color = color;
        text.text = message;
        displayEnd = Time.time + time;
        running = true;
        return onEnd;
    }

    public void Clear()
    {
        text.text = "";
        running = false;
    }

    private void Update()
    {
        if (!running)
            return;
        if (displayEnd <= Time.time)
        {
            text.text = "";
            running = false;
            onEnd?.Invoke();
        }
    }
}
