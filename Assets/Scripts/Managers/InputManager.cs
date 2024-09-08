using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput input;

    private static InputManager instance;

    private void Start()
    {
        input = new PlayerInput();
        GameManager.onRoundStart += HandleStart;
        GameManager.onRoundEnd += HandleEnd;

        input.InGame.TakeDiscard.performed += ctx => TakeDiscard();

        input.InGame.Swapcard1.performed += ctx => SwapCard(0);
        input.InGame.Swapcard2.performed += ctx => SwapCard(1);
        input.InGame.Swapcard3.performed += ctx => SwapCard(2);
        input.InGame.Swapcard4.performed += ctx => SwapCard(3);

        input.InGame.ViewSpoons.performed += ctx => ViewSpoons();
    }

    private void HandleStart(List<Player> _)
    {
        if (Player.localPlayer.isDead)
            return;
        input.InGame.Enable();
    }

    private void HandleEnd(Player _)
    {
        input.InGame.Disable();
    }

    private void TakeDiscard()
    {
        if (UIManager.viewingCard)
            UIManager.DiscardCard();
        else
            UIManager.TakeCard();
    }
    private void SwapCard(int card)
    {
        UIManager.SwapCard(card);
    }
    private void ViewSpoons()
    {
        UIManager.ViewSpoons();
    }

}
