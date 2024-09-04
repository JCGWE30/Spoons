using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image mainPanel;

    [SerializeField] private Button drawPile;
    [SerializeField] private Button card1;
    [SerializeField] private Button card2;
    [SerializeField] private Button card3;
    [SerializeField] private Button card4;
    [SerializeField] private Button discardPile;

    [SerializeField] private Button viewButton;
    [SerializeField] private Image viewCooldown;
    [SerializeField] private Image viewCooldownBar;

    [SerializeField] private Image viewTimer;
    [SerializeField] private Button takeSpoon;

    [SerializeField] private TMP_Text topText;

    public static UIManager instance;

    private Card drawCard;
    private bool viewingSpoons = false;
    private float lastToggled = -50;

    public List<string> topTexts = new List<string>();
    private float startTopText = 0;
    private bool showingTopText = false;

    public delegate void EndTopTextHandler();

    public EndTopTextHandler OnTopTextEnd;

    private void Start()
    {
        instance = this;
        mainPanel.gameObject.SetActive(false);
        SpoonsPlayer.onRoundStart += SetupRound;
        SpoonsPlayer.onRoundEnd += EndRound;
        if (SpoonsPlayer.roundStarted)
            SetupRound(SpoonsPlayer.playerCount);
    }

    private void SetupRound(int count)
    {
        drawPile.onClick.AddListener(delegate { DrawCard(); });

        card1.onClick.AddListener(delegate { ClickCard(0); });
        card2.onClick.AddListener(delegate { ClickCard(1); });
        card3.onClick.AddListener(delegate { ClickCard(2); });
        card4.onClick.AddListener(delegate { ClickCard(3); });

        discardPile.onClick.AddListener(delegate { DiscardCard(); });

        viewButton.onClick.AddListener(delegate { ToggleView(); });

        takeSpoon.onClick.AddListener(delegate { TakeSpoon(); });
        Debug.Log("Running Round");
        mainPanel.gameObject.SetActive(true);
    }

    private void EndRound()
    {
        drawCard = null;
        viewingSpoons = false;
        lastToggled = 0;
        mainPanel.gameObject.SetActive(false);
        viewTimer.gameObject.SetActive(false);
        takeSpoon.gameObject.SetActive(false);
    }

    private void DrawCard()
    {
        if (SpoonsPlayer.localInstance.GetDeck().cardCount == 0)
            return;
        if (drawCard != null)
            return;
        drawCard = SpoonsPlayer.localInstance.GetDeck().TakeCard();
    }

    private void DiscardCard()
    {
        if (drawCard != null)
        {
            SpoonsPlayer.localInstance.SendCard(drawCard);
            drawCard = null;
        }
    }

    private void ClickCard(int card)
    {
        if (drawCard != null)
        {
            SpoonsPlayer.localInstance.SwapCard(card, drawCard);
            drawCard = null;
        }
    }

    private void ToggleView()
    {
        if (lastToggled + Constants.SPOONS_TOGGLE_VIEW_COOLDOWN > Time.time)
            return;
        viewingSpoons = true;
        takeSpoon.gameObject.SetActive(true);
        viewTimer.gameObject.SetActive(true);
        mainPanel.gameObject.SetActive(false);
        lastToggled = Time.time;
    }

    private void TakeSpoon()
    {
        if (viewingSpoons != true)
            return;
        SpoonsPlayer.localInstance.TakeSpoonRpc();
    }

    private void Update()
    {
        if (topTexts.Count > 0)
        {
            float time = (float)NetworkManager.Singleton.ServerTime.Time; ;
            if (!showingTopText)
            {
                startTopText = time;
                topText.gameObject.SetActive(true);
                topText.text = topTexts[0];
                showingTopText = true;
            }
            else
            {
                if (startTopText + Constants.PLAYER_TOPTEXT_TIME < time)
                {
                    topTexts.RemoveAt(0);
                    showingTopText = false;
                    if (topTexts.Count == 0)
                    {
                        topText.gameObject.SetActive(false);
                        OnTopTextEnd?.Invoke();
                    }
                }
            }
        }

        if (!SpoonsPlayer.roundStarted)
            return;
        if (viewingSpoons)
        {
            float startOffset = Time.time - lastToggled;

            float percent = startOffset / Constants.SPOONS_VIEW_TIMER;

            viewTimer.fillAmount = Mathf.Max(0, 1 - percent);

            if (lastToggled + Constants.SPOONS_VIEW_TIMER < Time.time)
            {
                lastToggled = Time.time;
                viewingSpoons = false;
                takeSpoon.gameObject.SetActive(false);
                mainPanel.gameObject.SetActive(true);
                viewTimer.gameObject.SetActive(false);
            }
            return;
        }
        if (lastToggled + Constants.SPOONS_TOGGLE_VIEW_COOLDOWN > Time.time)
        {
            viewCooldown.gameObject.SetActive(true);

            float startOffset = Time.time - lastToggled;

            float percent = startOffset / Constants.SPOONS_TOGGLE_VIEW_COOLDOWN;

            viewCooldownBar.fillAmount = percent;
        }
        else
        {
            viewCooldown.gameObject.SetActive(false);
        }

        string drawtext = drawCard?.GetName() ?? SpoonsPlayer.localInstance.GetDeck().cardCount+"";
        drawPile.GetComponentInChildren<TMP_Text>().text = drawtext;

        card1.GetComponentInChildren<TMP_Text>().text = SpoonsPlayer.localInstance.GetHand().GetCard(0).GetName();
        card2.GetComponentInChildren<TMP_Text>().text = SpoonsPlayer.localInstance.GetHand().GetCard(1).GetName();
        card3.GetComponentInChildren<TMP_Text>().text = SpoonsPlayer.localInstance.GetHand().GetCard(2).GetName();
        card4.GetComponentInChildren<TMP_Text>().text = SpoonsPlayer.localInstance.GetHand().GetCard(3).GetName();

        discardPile.GetComponentInChildren<TMP_Text>().text = "Discard Pile";
    }
}
