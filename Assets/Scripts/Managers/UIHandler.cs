using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
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

    [SerializeField] private Sprite drawBack;

    [SerializeField] private TMP_Text spoonCount;

    public static UIHandler instance;
    public string topTextValue;

    public bool drawingCard = false;
    public Sprite drawSprite;
    public string letters;
    public bool renderDraw;
    public Sprite[] cardSprites = new Sprite[4];

    private bool viewingSpoons = false;
    private float viewingCooldown = -50f;

    private void Start()
    {
        mainPanel.gameObject.SetActive(false);
        instance = this;
        GameManager.onRoundStart += StartRoundUI;
        GameManager.onRoundEnd += EndRoundUI;
        Player.onIsSafe += () => { EndRoundUI(null); };

        drawPile.onClick.AddListener(delegate { TakeCard(); });

        card1.onClick.AddListener(delegate { SwapCard(0); });
        card2.onClick.AddListener(delegate { SwapCard(1); });
        card3.onClick.AddListener(delegate { SwapCard(2); });
        card4.onClick.AddListener(delegate { SwapCard(3); });

        discardPile.onClick.AddListener(delegate { DiscardCard(); });

        viewButton.onClick.AddListener(delegate { ViewSpoons(); });
        takeSpoon.onClick.AddListener(delegate { TakeSpoon(); });
    }

    private void Update()
    {
        topText.text = topTextValue;
        if (!GameManager.roundStarted)
            return;
        if (viewingSpoons)
        {
            float starttime = Time.time - viewingCooldown;

            float percent = starttime / Constants.SPOONS_TOGGLE_VIEW_COOLDOWN;

            if (percent >= 1)
            {
                viewingSpoons = false;
                viewingCooldown = Time.time;
                viewTimer.gameObject.SetActive(false);
                takeSpoon.gameObject.SetActive(false);
                mainPanel.gameObject.SetActive(true);
                viewButton.gameObject.SetActive(true);
                return;
            }

            viewTimer.fillAmount = 1 - percent;
            return;
        }
        if (viewingCooldown + Constants.SPOONS_TOGGLE_VIEW_COOLDOWN > Time.time)
        {
            viewCooldown.gameObject.SetActive(true);
            float starttime = Time.time - viewingCooldown;

            float percent = starttime / Constants.SPOONS_TOGGLE_VIEW_COOLDOWN;

            viewCooldownBar.fillAmount = percent;
        }
        else
        {
            viewCooldown.gameObject.SetActive(false);
        }


        drawPile.gameObject.SetActive(renderDraw);
        spoonCount.text = letters;
        if (!drawingCard)
        {
            drawPile.image.sprite = drawBack;
        }
        else
        {
            drawPile.image.sprite = drawSprite;
        }

        card1.image.sprite = cardSprites[0];
        card2.image.sprite = cardSprites[1];
        card3.image.sprite = cardSprites[2];
        card4.image.sprite = cardSprites[3];
    }

    private void StartRoundUI(List<Player> participants)
    {
        mainPanel.gameObject.SetActive(true);
        viewButton.gameObject.SetActive(true);
    }

    private void EndRoundUI(Player loser)
    {
        mainPanel.gameObject.SetActive(false);
        viewButton.gameObject.SetActive(false);
        viewTimer.gameObject.SetActive(false);
        takeSpoon.gameObject.SetActive(false);
        viewingSpoons = false;
    }

    private void TakeCard()
    {
        UIManager.TakeCard();
    }

    private void SwapCard(int card)
    {
        UIManager.SwapCard(card);
    }

    private void DiscardCard()
    {
        UIManager.DiscardCard();
    }

    public void ViewSpoons()
    {
        if (viewingCooldown + Constants.SPOONS_TOGGLE_VIEW_COOLDOWN > Time.time)
            return;
        viewingSpoons = true;
        viewingCooldown = Time.time;

        mainPanel.gameObject.SetActive(false);
        viewButton.gameObject.SetActive(false);
        takeSpoon.gameObject.SetActive(true);
        viewTimer.gameObject.SetActive(true);
    }

    private void TakeSpoon()
    {
        UIManager.TakeSpoon();
    }
}
