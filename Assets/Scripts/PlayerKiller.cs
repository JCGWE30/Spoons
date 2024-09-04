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
    [SerializeField] private NetworkObject deadBody;

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
        yield return new WaitForSeconds(3f);
        DimScreenRpc();
        SpoonsPlayer.localInstance.RemovePlayer(player);
        player.DeactivateRpc();
        NetworkObject body = Instantiate(deadBody);
        body.transform.position = player.gameObject.transform.position;
        Vector3 pos = body.transform.position;
        body.transform.position = new Vector3(pos.x, pos.y - 0.7f, pos.z);
        body.Spawn();
        yield return new WaitForSeconds(3f);
        GunshotRpc();
        yield return new WaitForSeconds(3f);
        UnDimScreenRpc();
        yield return new WaitForSeconds(2f);
        SpoonsPlayer.StartRound();
    }

    private void PlayAudio(AudioClip clip)
    {
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
