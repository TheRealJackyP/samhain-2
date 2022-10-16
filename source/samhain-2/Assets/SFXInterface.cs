using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXInterface : MonoBehaviour
{
    public static SFXInterface Instance;

    public AudioClip ShootSFX;
    public AudioClip ArmorSFX;
    public AudioClip DrawSFX;
    public AudioClip DiscardSFX;
    public AudioClip EnemySFX;

    public AudioClip BattleMusic;
    public AudioClip GameMusic;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioManager.Instance.PlaySFX(clip);
    }
}
