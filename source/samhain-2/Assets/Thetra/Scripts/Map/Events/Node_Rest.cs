using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Rest : BaseMapNode
{
    public GameObject[] EventPrefabs;

    override protected void Node()
    {

        Instantiate(EventPrefabs[Random.Range(0, EventPrefabs.Length)]);
        Refill();

    }


    void Refill()
    {
        //refill exhaustet stats

    }

}
