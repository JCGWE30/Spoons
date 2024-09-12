using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class SkinWatcher : MonoBehaviour
{
    public static SkinObject activeSkin;

    [SerializeField] private SkinObject skin;
    [SerializeField] private MonoBehaviour caller;

    private delegate void SkinEquipHandler();
    private static SkinEquipHandler onSkinEquipped;

    private bool canEquip = false;
    private TMP_Text label;
    private Button equipButton;
    private GameObject skinShowcase;

    private async void Start()
    {
        label = transform.Find("SkinLabel").gameObject.GetComponent<TMP_Text>();
        equipButton = transform.Find("EquipButton").gameObject.GetComponent<Button>();
        skinShowcase = transform.Find("SkinShowcase").gameObject;

        ServicesHandler.onServiceStart += async () =>
        {
            ISkinCaller skinCaller = caller as ISkinCaller;
            Debug.Log(skinCaller);
            canEquip = await skinCaller.canEquip(AuthenticationService.Instance.PlayerId);
        };

        Debug.Log("Setting button " + equipButton);
        equipButton.onClick.AddListener(AttemptEquip);

        label.text = skin.skinName;
        onSkinEquipped += () =>
        {
            string equipText = "";

            if (activeSkin == skin)
                equipText = Constants.SKINS_UNEQUIP;
            else if (canEquip)
                equipText = Constants.SKINS_EQUIP;
            else
                equipText = Constants.SKINS_LOCKED;

            equipButton.GetComponentInChildren<TMP_Text>().text = equipText;
        };
    }

    private void AttemptEquip()
    {
        if (canEquip)
        {
            if (activeSkin == skin)
            {
                activeSkin = null;
                SkinsHandler.DisplayMessage(Constants.SKINS_RESPONSE_UNEQUIP, false);
            }
            else
            {
                activeSkin = skin;
                SkinsHandler.DisplayMessage(Constants.SKINS_RESPONSE_EQUIP, false);
            }
            onSkinEquipped?.Invoke();
        }
        else
        {
            SkinsHandler.DisplayMessage(skin.unlockReqs,true);
        }
    }
}
