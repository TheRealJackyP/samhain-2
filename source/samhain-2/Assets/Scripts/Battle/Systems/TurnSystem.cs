using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TurnSystem : MonoBehaviour
{
    public UnityEvent<GameObject, GameObject> OnTurnStart;
    public UnityEvent<GameObject> OnTurnEnd;

    public List<GameObject> TurnSequence = new();
    public int CurrentTurnIndex;

    public void Start()
    {
        if(TurnSequence.Any())
            Init();
    }

    public void Init()
    {
        OnTurnStart.Invoke(null, TurnSequence[CurrentTurnIndex]);
    }

    public void StartNextTurn()
    {
        var currentTurn = TurnSequence[CurrentTurnIndex];
        OnTurnEnd.Invoke(TurnSequence[CurrentTurnIndex]);
        CurrentTurnIndex = (CurrentTurnIndex + 1) % TurnSequence.Count;
        OnTurnStart.Invoke(currentTurn, TurnSequence[CurrentTurnIndex]);
    }

    public GameObject GetCurrentTurn()
    {
        return TurnSequence[CurrentTurnIndex];
    }
}