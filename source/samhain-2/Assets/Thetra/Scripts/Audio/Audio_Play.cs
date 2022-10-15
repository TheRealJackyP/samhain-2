using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Play : MonoBehaviour
{
   public AudioClip clip;
    public bool isMusic;


    private void Start()
    {
        
        if(isMusic)
            AudioManager.Instance.PlayMusic(clip);
        else
            AudioManager.Instance.PlaySFX(clip);
    }


}
