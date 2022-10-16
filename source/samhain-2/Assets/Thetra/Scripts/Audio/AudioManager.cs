using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private void Awake()
    {
     
        Instance = this;


    }

    public AudioSource Music, SFX;


    public void PlayMusic(AudioClip clip)
    {
        Music.clip = null;
        Music.clip = clip;
        Music.PlayDelayed(1);
    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
      
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }



}
