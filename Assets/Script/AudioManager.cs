using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public void Awake()
    {
        instance = this;
    }

    public void PlaySFX(AudioClip audio, float volume = 1f)
    {
        StartCoroutine(PlaySFXCoroutine(audio, volume));
    }

    IEnumerator PlaySFXCoroutine(AudioClip audio, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length * 2);

        Destroy(audioSource);
    }
}
