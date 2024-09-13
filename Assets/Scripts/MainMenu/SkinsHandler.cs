using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinsHandler : MonoBehaviour
{
    [SerializeField] private Image skinsPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private HideAfterTime errorMessage;
    [SerializeField] private List<SkinObject> skins;
    [SerializeField] private GameObject preview;
    [SerializeField] private Material defaultMat;
    [SerializeField] private TMP_Text titleText;

    private static SkinsHandler instance;

    public static string ActiveSkin { get { return SkinWatcher.activeSkin?.skinName ?? "none"; } }

    private void Start()
    {
        skinsPanel.gameObject.SetActive(false);
        instance = this;
        backButton.onClick.AddListener(ExitSkins);
        SkinWatcher.onSkinEquipped += () =>
        {
            UpdateTitle();
            if (SkinWatcher.activeSkin == null)
                preview.GetComponent<MeshRenderer>().material = defaultMat;
            else
                preview.GetComponent<MeshRenderer>().material = SkinWatcher.activeSkin.texture;
        };
    }

    private void Update()
    {
        preview.transform.Rotate(0, 0.1f, 0);
    }

    public static void EnterSkins()
    {
        instance.skinsPanel.gameObject.SetActive(true);
        instance.LoadSkin();
        instance.UpdateTitle();
    }

    public static void DisplayMessage(string message,bool error)
    {
        instance.errorMessage.Display(message,3f, error ? Color.red : Color.green);
    }

    private void ExitSkins()
    {
        skinsPanel.gameObject.SetActive(false);
        MenuHandler.BackToMenu();
        instance.titleText.text = "SPOONS";
    }

    private void UpdateTitle()
    {
        PlayerPrefs.SetString(Constants.PREFS_ACTIVESKIN, ActiveSkin);
        if (ActiveSkin == "none")
        {
            titleText.text = "Default Skin";
        }
        else
        {
            titleText.text = ActiveSkin;
        }
    }

    private void LoadSkin()
    {
        if (!PlayerPrefs.HasKey(Constants.PREFS_ACTIVESKIN))
            return;
        string skinName = PlayerPrefs.GetString(Constants.PREFS_ACTIVESKIN);
        if (skinName == "none")
            return;
        foreach (SkinObject skin in skins)
        {
            if (skin.skinName == skinName)
            {
                SkinWatcher.activeSkin = skin;
                UpdateTitle();
                SkinWatcher.onSkinEquipped?.Invoke();
                return;
            }
        }
    }

    public static Material GetMaterial(string name)
    {
        foreach(SkinObject skin in instance.skins)
        {
            if(skin.skinName == name)
            {
                return skin.texture;
            }
        }
        return null;
    }

}
