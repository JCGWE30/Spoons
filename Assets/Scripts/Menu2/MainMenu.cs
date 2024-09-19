using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playbutton;

    private void Start()
    {
        playbutton.onClick.AddListener(ToPlay);
    }

    private void ToPlay()
    {
        if (MenuTransition.inMotion)
            return;

        MenuTransition.StartMove(TransitionMenu.PlayMenu, 1f);
    }
}
