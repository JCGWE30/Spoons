using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerKiller : NetworkBehaviour
{
    [SerializeField] private Image screenCover;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lightSwitch;
    [SerializeField] private AudioClip gunShot;

    public static PlayerKiller instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void KillPlayer(SpoonsPlayer player)
    {
        StartCoroutine(KillSequence(player));
    }

    private IEnumerator KillSequence(SpoonsPlayer player)
    {
        yield return new WaitForSeconds(2f);
        DimScreenRpc();
        yield return new WaitForSeconds(1f);
        GunshotRpc();
        yield return new WaitForSeconds(2f);
        UnDimScreenRpc();
    }

    private void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }


    [Rpc(SendTo.NotServer)]
    private void DimScreenRpc()
    {
        screenCover.gameObject.SetActive(true);
        PlayAudio(lightSwitch);
    }

    [Rpc(SendTo.NotServer)]
    private void UnDimScreenRpc()
    {
        screenCover.gameObject.SetActive(false);
        PlayAudio(lightSwitch);
    }

    [Rpc(SendTo.NotServer)]
    private void GunshotRpc()
    {
        PlayAudio(gunShot);
    }
}
