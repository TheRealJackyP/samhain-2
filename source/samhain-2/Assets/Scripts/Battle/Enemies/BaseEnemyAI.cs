using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BaseEnemyAI : MonoBehaviour
{
    public int Attack;
    public List<GameObject> Characters;
    public GameObject NextTarget;

    public bool TurnComplete;
    public TurnSystem TurnSystem;

    private Coroutine DoTurnInstance;

    public UnityEvent OnAnimateAttack = new();
    public UnityEvent OnAnimateAttackComplete = new();

    private void Start()
    {
        NextTarget = Characters[Random.Range(0, Characters.Count)];
    }

    private void OnDestroy()
    {
        OnAnimateAttack.RemoveAllListeners();
        OnAnimateAttackComplete.RemoveAllListeners();
    }

    public void InvokeAnimateAttackComplete()
    {
        OnAnimateAttackComplete.Invoke();
        TurnComplete = true;
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
        if(!target.GetComponent<EntityHealth>().IsDead)
            target.GetComponent<EntityHealth>().TakeDamage(Attack);
        else
        {
            var livingCharacters = Characters.Where(element => !element.GetComponent<EntityHealth>().IsDead).ToList();
            if (livingCharacters.Any())
            {
                livingCharacters.First().GetComponent<EntityHealth>().TakeDamage(Attack);
            }
        }
        AudioManager.Instance.PlaySFX(SFXInterface.Instance.EnemySFX);
        OnAnimateAttack.Invoke();
    }

    public IEnumerator DoTurn()
    {
        if(NextTarget != null)
            PerformAttack(NextTarget);
        while (!TurnComplete)
        {
            yield return null;
        }

        var elapsedTime = 0f;
        while (elapsedTime < .5f)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }
            var livingCharacters = Characters.Where(element => !element.GetComponent<EntityHealth>().IsDead).ToList();
        NextTarget = livingCharacters.Any() ? livingCharacters[Random.Range(0, livingCharacters.Count)]: null;
        TurnSystem.StartNextTurn();
    }
}