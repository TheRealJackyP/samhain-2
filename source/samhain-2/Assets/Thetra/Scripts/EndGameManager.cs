using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    
    public GameObject EndEvent;

    void Start()
    {
        if(MapManager.instance.isLastBattle)
            Instantiate(EndEvent);
    }

}
