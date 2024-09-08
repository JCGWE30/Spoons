using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource player;
    [SerializeField] private AudioClip music;

    private void Start()
    {
        GameManager.onRoundStart += p => StartMusic();
        GameManager.onRoundEnd += p => EndMusic();
    }

    private void StartMusic()
    {
        player.volume = SettingsHandler.musicVolume / 100f;
        player.clip = music;
        player.loop = true;
        player.Play();
    }
    private void EndMusic()
    {
        player.Stop();
        player.loop = true;
    }
}
