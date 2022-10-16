using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node_Battle : BaseMapNode
{ 

   override protected void Node()
    {
        SceneManger.instance.ChangeScene(2);
    }

}
