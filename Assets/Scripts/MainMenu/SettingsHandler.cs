using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Image settingsPanel;

    [SerializeField] private TMP_Text sfxDisplay;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TMP_Text musicDisplay;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip sfxTest;
    [SerializeField] private AudioClip musicTest;

    public static int musicVolume = 100;
    public static int sfxVolume = 100;

    private static SettingsHandler instance;

    private void Start()
    {
        instance = this;
        musicSlider.value = PlayerPrefs.HasKey(Constants.PREFS_MUSICVOLUME)
            ? PlayerPrefs.GetInt(Constants.PREFS_MUSICVOLUME) : musicVolume;

        sfxSlider.value = PlayerPrefs.HasKey(Constants.PREFS_SFXVOLUME)
            ? PlayerPrefs.GetInt(Constants.PREFS_SFXVOLUME) : sfxVolume; ;

        settingsPanel.gameObject.SetActive(false);
        backButton.onClick.AddListener(Back);
        musicSlider.onValueChanged.AddListener(delegate { MusicChange(); });
        sfxSlider.onValueChanged.AddListener(delegate { EffectChange(); });
    }

    private void Update()
    {
        musicDisplay.text = "Music Volume: " + musicSlider.value;
        sfxDisplay.text = "Effects Volume: " + sfxSlider.value;
    }

    private void MusicChange()
    {
        musicVolume = (int)musicSlider.value;
        PlayerPrefs.SetInt(Constants.PREFS_MUSICVOLUME, musicVolume);
        PlayAudio(musicTest,musicVolume);
    }
    private void EffectChange()
    {
        sfxVolume = (int)sfxSlider.value;
        PlayerPrefs.SetInt(Constants.PREFS_SFXVOLUME, sfxVolume);
        PlayAudio(sfxTest,sfxVolume);
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

    private void PlayAudio(AudioClip clip,int vol)
    {
        audioPlayer.volume = vol / 100f;
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }
}
