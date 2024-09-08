using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EliminationManager : NetworkBehaviour
{
    [SerializeField] private Image screenCover;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lightSwitch;
    [SerializeField] private AudioClip gunShot;
    [SerializeField] private NetworkObject deadBody;

    public static EliminationManager instance;
    void Start()
    {
        instance = this;
    }

    public void KillPlayer(Player player)
    {
        StartCoroutine(KillSequence(player));
    }

    private IEnumerator KillSequence(Player player)
    {
        yield return new WaitForSeconds(3f);
        DimScreenRpc();
        GameManager.EliminatePlayer(player.OwnerClientId);
        yield return new WaitForSeconds(3f);
        GunshotRpc();
        yield return new WaitForSeconds(3f);
        UnDimScreenRpc();
        yield return new WaitForSeconds(2f);
        GameManager.StartRound();
    }

    private void PlayAudio(AudioClip clip)
    {
        audioSource.volume = SettingsHandler.sfxVolume / 100f;
        audioSource.clip = clip;
        audioSource.Play();
    }


    [Rpc(SendTo.Everyone)]
    private void DimScreenRpc()
    {
        screenCover.gameObject.SetActive(true);
        PlayAudio(lightSwitch);
    }

    [Rpc(SendTo.Everyone)]
    private void UnDimScreenRpc()
    {
        screenCover.gameObject.SetActive(false);
        PlayAudio(lightSwitch);
    }

    [Rpc(SendTo.Everyone)]
    private void GunshotRpc()
    {
        PlayAudio(gunShot);
    }
}
