using System;
using UnityEngine;
using UnityEngine.Events;

public class TargetingSystem : MonoBehaviour
{
    public GameObject ActiveTarget;
    public GameObject ActiveTurn;
    public GameObject ActiveCardAnchor;
    public GameObject ActiveCard;

    public UnityEvent<GameObject> OnTargetClicked = new();
    public UnityEvent<GameObject> OnTargetUnClicked = new();

    public void UpdateTurn(GameObject LastTurn, GameObject NextTurn)
    {
        ActiveTarget = null;
        ActiveTurn = NextTurn;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && ActiveTarget is not null)
        {
            OnTargetClicked.Invoke(ActiveTarget);
        }
        
        else if (Input.GetMouseButtonDown(1) && ActiveTarget is not null)
        {
            OnTargetUnClicked.Invoke(ActiveCard);
        }
    }
}