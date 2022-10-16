using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnSystem : MonoBehaviour
{
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
        for (var i = 0; i <= TurnSequence.Count && !TurnSequence[CurrentTurnIndex].activeSelf; ++i)
        {
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
    }

    public GameObject GetCurrentTurn()
    {
        return TurnSequence[CurrentTurnIndex];
    }
}