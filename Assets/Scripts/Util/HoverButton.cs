using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverButton : Selectable
{
    public delegate void HoverButtonDelegate();
    public HoverButtonDelegate onHover;
    public HoverButtonDelegate onUnHover;

    private bool highlightState = false;
    private void Update()
    {
        if (IsHighlighted() != highlightState)
        {
            highlightState = IsHighlighted();

            if (highlightState)
            {
                onHover?.Invoke();
            }
            else
            {
                onUnHover?.Invoke();    
            }
        }
    }

    public void Wipe()
    {
        onHover = null;
        onUnHover = null;
    }
}
