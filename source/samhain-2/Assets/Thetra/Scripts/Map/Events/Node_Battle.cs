using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node_Battle : BaseMapNode
{
    public int BattleSceneID = 2;
    override protected void Node()
    {
        SceneManger.instance.ChangeScene(BattleSceneID);
    }
}
