using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public static Cam instance;
    
    void Awake()
    {
        instance = this;
    }



    Vector3 offset = new Vector3(0f, 0f, -10f);
    Vector3 speed = Vector3.zero;

    public float smooth = 0.25f;
    public Transform node;

    void Update()
    {
        if(node == null)    
            return;

        Vector3 target = node.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref speed, smooth);
    }
}

