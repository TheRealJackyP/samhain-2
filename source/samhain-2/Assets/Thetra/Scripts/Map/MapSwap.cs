using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSwap : MonoBehaviour
{
    public GameObject MapA, MapB; 

    private void Awake()
    {
        if(MapManager.instance.MapB)
        {
            MapA.SetActive(false);
            MapB.SetActive(true);
        }
        else
        {
            MapA.SetActive(true);
            MapB.SetActive(false);
        }
    }
}
