using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public void PlaySFX(AudioClip sfxClip)
    {
        sfxSource.clip = sfxClip;
        sfxSource.PlayOneShot(sfxClip);
    }

    public void PlayMusic(AudioClip musicToPlay)
    {
        musicSource.Stop();
        musicSource.clip = musicToPlay;
        musicSource.Play();
    }

    public void ToggleSFX()
    {
        sfxSource.enabled = !sfxSource.enabled;
    }

    public void ToggleMusic()
    {
        musicSource.enabled = !musicSource.enabled;
    }
    
}
