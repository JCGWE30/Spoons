using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinsHandler : MonoBehaviour
{
    [SerializeField] private Image skinsPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private List<SkinObject> skins;

    private static SkinsHandler instance;

    public static string ActiveSkin { get { return SkinWatcher.activeSkin?.skinName ?? "none"; } }

    private void Start()
    {
        skinsPanel.gameObject.SetActive(false);
        instance = this;
        backButton.onClick.AddListener(ExitSkins);
    }
    public static void EnterSkins()
    {
        instance.skinsPanel.gameObject.SetActive(true);
    }

    public static void DisplayMessage(string message,bool error)
    {
        instance.errorMessage.color = error ? Color.red : Color.green;
        instance.errorMessage.text = message;
        instance.StartCoroutine(instance.HideError());
    }
    
    private IEnumerator HideError()
    {
        yield return new WaitForSeconds(3f);
        errorMessage.text = "";
    }

    private void ExitSkins()
    {
        skinsPanel.gameObject.SetActive(false);
        MenuHandler.BackToMenu();
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
