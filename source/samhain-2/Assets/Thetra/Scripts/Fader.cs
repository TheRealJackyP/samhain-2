using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fader : MonoBehaviour
{
    public static Fader instance;
    private void Awake()
    {
        instance = this;
    }


    public Animator anim;

    public void FadeOut()
    {
        anim.SetBool("Black", true);
    }

    public void FadeIn()
    {
        anim.SetBool("Black", false);
    }


}
