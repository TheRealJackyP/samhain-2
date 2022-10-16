using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Event : BaseMapNode
{
    public GameObject[] EventPrefabs;



    override protected void Node()
    {
        StartCoroutine(EventStart());
    }

    IEnumerator EventStart()
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(EventPrefabs[Random.Range(0, EventPrefabs.Length)]);
    }

}
