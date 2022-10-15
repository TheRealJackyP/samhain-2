using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Event : BaseMapNode
{
    public GameObject[] EventPrefabs;



    override protected void Node()
    {
        Instantiate(EventPrefabs[Random.Range(0, EventPrefabs.Length)]);
    }
}
