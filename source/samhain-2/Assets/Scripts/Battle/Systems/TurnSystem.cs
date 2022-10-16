using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class TurnSystem : MonoBehaviour
{
    public Button TurnButton;
    public UnityEvent<GameObject, GameObject> OnTurnStart;
    public UnityEvent<GameObject> OnTurnEnd;

    public List<GameObject> TurnSequence = new();
    public int CurrentTurnIndex;

    public bool WaitForEvent;
    public bool EventComplete;
    public UnityEvent OnTurnStartAnimationComplete;

    public Coroutine StartTurnInstance;
    public void Start()
    {
        // if(TurnSequence.Any())
        //     Init();
    }
    

    public IEnumerator DoStartTurn(GameObject currentTurn)
    {
        EventComplete = false;
        OnTurnStartAnimationComplete.AddListener(() => EventComplete = true);
        while (!EventComplete)
        {
            yield return null;
        }
        
        OnTurnStartAnimationComplete.RemoveListener(() => EventComplete = true);
        OnTurnStart.Invoke(currentTurn, TurnSequence[CurrentTurnIndex]);
        if (GetCurrentTurn().TryGetComponent<BaseEnemyAI>(out var _))
        {
            TurnButton.interactable = false;
        }
        else
        {
            TurnButton.interactable = true;
        }
    }

    public void Init()
    {
        OnTurnStart.Invoke(null, TurnSequence[CurrentTurnIndex]);
    }

    public void StartNextTurn()
    {
        Dragger.DragTarget = null;
        var currentTurn = TurnSequence[CurrentTurnIndex];
        OnTurnEnd.Invoke(TurnSequence[CurrentTurnIndex]);
        CurrentTurnIndex = (CurrentTurnIndex + 1) % TurnSequence.Count;
        for (var i = 0; i <= TurnSequence.Count; ++i)
        {
            if (TurnSequence[CurrentTurnIndex].activeSelf &&
                !TurnSequence[CurrentTurnIndex].GetComponent<EntityHealth>().IsDead)
            {
                break;
            }
            if (i == TurnSequence.Count) throw new Exception("Unable to start a turn!");
            CurrentTurnIndex = (CurrentTurnIndex + 1) % TurnSequence.Count;
        }

        if(WaitForEvent && EventComplete)
            OnTurnStart.Invoke(currentTurn, TurnSequence[CurrentTurnIndex]);
        else if (!WaitForEvent)
        {
            OnTurnStart.Invoke(currentTurn, TurnSequence[CurrentTurnIndex]);
        }
        else
        {
            StartTurnInstance = StartCoroutine(DoStartTurn(currentTurn));
        }
        if (GetCurrentTurn().TryGetComponent<BaseEnemyAI>(out var _))
        {
            TurnButton.interactable = false;
        }
        else
        {
            TurnButton.interactable = true;
        }
    }

    public GameObject GetCurrentTurn()
    {
        return TurnSequence[CurrentTurnIndex];
    }
}