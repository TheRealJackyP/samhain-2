using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(MapManager.instance.gameObject);
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
    public bool MapB;
    public int CurrentNode;
    public List<GameObject> AllNodes = new List<GameObject>();
    public List<int> id = new List<int>();
    public bool isLastBattle;

    public GameObject StartEvent;
    bool started;
    public void Initiate()
    {
        StartCoroutine(InitiateCO());
    }
    public void MapAbool(bool MapChange)
    {
        MapB = !MapB;
    }
    IEnumerator InitiateCO()
    {
        yield return new WaitForSeconds(0.1f);
        if (AllNodes.Count != 0)
        {
            OnLoad();
            Debug.Log("SearchPosition");
            for (int i = 0; i < AllNodes.Count; i++)
            {
                if (AllNodes[i].GetComponent<NodeID>().ID == CurrentNode)
                {
                    if (AllNodes[i].GetComponent<Node_Battle>() != null)
                        Cam.instance.node = AllNodes[i].GetComponent<Node_Battle>().CurserPos;

                    if (AllNodes[i].GetComponent<Node_Event>() != null)
                        Cam.instance.node = AllNodes[i].GetComponent<Node_Event>().CurserPos;

                    if (AllNodes[i].GetComponent<Node_Rest>() != null)
                        Cam.instance.node = AllNodes[i].GetComponent<Node_Rest>().CurserPos;
                }
            }
        }
    }


    private void OnLoad()
    {
        if(isLastBattle)
        {
            Debug.Log("GameEnd");
            SceneManger.instance.ChangeScene(3);
        }

        Debug.Log("OnLoad");
        foreach  (GameObject Node in AllNodes)
        {
            if (Node != null)
            {
                for (int i = 0; i < id.Count; i++)
                {
                    if (id[i] == Node.GetComponent<NodeID>().ID)
                    {
                        Node.GetComponent<BaseMapNode>().picked = true;
                        Node.GetComponent<BaseMapNode>().Unlocked = true;

                        if(id[i] == CurrentNode)
                        {
                            if (Node.GetComponent<Node_Battle>() != null)
                                Cam.instance.node = Node.GetComponent<Node_Battle>().CurserPos;

                            if (Node.GetComponent<Node_Event>() != null)
                                Cam.instance.node = Node.GetComponent<Node_Event>().CurserPos;

                            if (Node.GetComponent<Node_Rest>() != null)
                                Cam.instance.node = Node.GetComponent<Node_Rest>().CurserPos;
                        }

                    }
                }
            }
          
        }

        if(CurrentNode == 0 && !started)
        {
            started = true;
            Instantiate(StartEvent);
        }
       


    }

    public void LastBattle()
    {
        isLastBattle = true;
    }
}
