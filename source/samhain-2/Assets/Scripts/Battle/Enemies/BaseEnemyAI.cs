using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    public int Attack;
    public List<GameObject> Characters;
    public GameObject NextTarget;

    public bool TurnComplete;
    public TurnSystem TurnSystem;

    private Coroutine DoTurnInstance;

    private void Start()
    {
        NextTarget = Characters[Random.Range(0, Characters.Count)];
    }

    public void PerformTurn(GameObject pastTurn, GameObject currentTurn)
    {
        if (currentTurn == gameObject)
        {
            TurnComplete = false;
            if (DoTurnInstance != null) StopCoroutine(DoTurnInstance);

            DoTurnInstance = StartCoroutine(DoTurn());
        }
    }

    public void PerformAttack(GameObject target)
    {
        target.GetComponent<EntityHealth>().TakeDamage(Attack);
    }

    public IEnumerator DoTurn()
    {
        PerformAttack(NextTarget);
        while (!TurnComplete) yield return null;

        NextTarget = Characters[Random.Range(0, Characters.Count)];
        TurnSystem.StartNextTurn();
    }
}