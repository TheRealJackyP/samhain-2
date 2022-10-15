using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseMapNode : MonoBehaviour
{
    [SerializeField] bool StartNode;
    [SerializeField] Color pickedcolor;    
    public bool Unlocked, picked;

    public Transform CurserPos;

    [Header("ConnectedNodes")]
    public GameObject[] unlockedNodes;
    public GameObject[] lockedNodes;

    private void Start()
    {
        MapManager.instance.AllNodes.Add(gameObject);
        //if (StartNode)
        //{
        //    Cam.instance.node = CurserPos;
        //    for (int i = 0; i < unlockedNodes.Length; i++)
        //    {

        //        if (unlockedNodes[i].GetComponent<Node_Battle>() != null)
        //            unlockedNodes[i].GetComponent<Node_Battle>().Unlocked = true;

        //        if (unlockedNodes[i].GetComponent<Node_Event>() != null)
        //            unlockedNodes[i].GetComponent<Node_Event>().Unlocked = true;

        //        if (unlockedNodes[i].GetComponent<Node_Rest>() != null)
        //            unlockedNodes[i].GetComponent<Node_Rest>().Unlocked = true;
        //    }
        //}
        if(MapManager.instance.CurrentNode == GetComponent<NodeID>().ID)
        {
            picked = true;
            OnNodeSelect();
        }
    }


    protected virtual void Node()
    {
       
    }

    public void OnNodeSelect()
    {
        if (!picked)
        {
            Node();
            MapManager.instance.id.Add(GetComponent<NodeID>().ID);
            MapManager.instance.CurrentNode = GetComponent<NodeID>().ID;
            picked = true;
        }
        if (MapManager.instance.CurrentNode == GetComponent<NodeID>().ID)
        {
            for (int i = 0; i < unlockedNodes.Length; i++)
            {

                if (unlockedNodes[i].GetComponent<Node_Battle>() != null)
                    unlockedNodes[i].GetComponent<Node_Battle>().Unlocked = true;

                if (unlockedNodes[i].GetComponent<Node_Event>() != null)
                    unlockedNodes[i].GetComponent<Node_Event>().Unlocked = true;

                if (unlockedNodes[i].GetComponent<Node_Rest>() != null)
                    unlockedNodes[i].GetComponent<Node_Rest>().Unlocked = true;
            }

            for (int j = 0; j < lockedNodes.Length; j++)
            {
                if (lockedNodes[j].GetComponent<Node_Battle>() != null)
                    lockedNodes[j].GetComponent<Node_Battle>().Unlocked = false;

                if (lockedNodes[j].GetComponent<Node_Event>() != null)
                    lockedNodes[j].GetComponent<Node_Event>().Unlocked = false;

                if (lockedNodes[j].GetComponent<Node_Rest>() != null)
                    lockedNodes[j].GetComponent<Node_Rest>().Unlocked = false;
            }
        }
        Cam.instance.node = CurserPos;
        
    }

    


    private void Update()
    {
        GetComponent<Button>().interactable = Unlocked;

        if (picked)
            GetComponent<Image>().color = pickedcolor;
    }


}
