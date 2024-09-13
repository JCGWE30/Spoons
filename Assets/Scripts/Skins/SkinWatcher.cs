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

    public delegate void SkinEquipHandler();
    public static SkinEquipHandler onSkinEquipped;

    private bool canEquip = false;
    private Button equipButton;
    private GameObject skinShowcase;

    private async void Awake()
    {
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

        onSkinEquipped += () =>
        {
            if (activeSkin == skin)
            {
                if (!(canEquip||Constants.DEBUG_MODE))
                {
                    activeSkin = null;
                    onSkinEquipped?.Invoke();
                    return;
                }
            }
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

    private void Update()
    {
        skinShowcase.transform.Rotate(0, 0.1f, 0);
    }

    private void AttemptEquip()
    {
        if (canEquip||Constants.DEBUG_MODE)
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
