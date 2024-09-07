using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text volumeDisplay;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Image settingsPanel;

    public static int volume { get { return (int) instance.volumeSlider.value; } }

    private static SettingsHandler instance;

    private void Start()
    {
        instance = this;
        volumeSlider.value = 0;
        settingsPanel.gameObject.SetActive(false);
        backButton.onClick.AddListener(Back);
    }

    private void Update()
    {
        volumeDisplay.text = "Game Volume: " + volumeSlider.value;
    }

    public static void EnterSettings()
    {
        instance.settingsPanel.gameObject.SetActive(true);
    }

    private void Back()
    {
        settingsPanel.gameObject.SetActive(false);
        MenuHandler.BackToMenu();
    }
}
