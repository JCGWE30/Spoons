using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesHandler : MonoBehaviour
{
    [SerializeField] private Image rulesPanel;
    [SerializeField] private Button backButton;

    private static RulesHandler instance;

    private void Start()
    {
        instance = this;
        rulesPanel.gameObject.SetActive(false);
        backButton.onClick.AddListener(Back);
    }

    public static void EnterRules()
    {
        instance.rulesPanel.gameObject.SetActive(true);
    }

    private void Back()
    {
        MenuHandler.BackToMenu();
        instance.rulesPanel.gameObject.SetActive(false);
    }
}
