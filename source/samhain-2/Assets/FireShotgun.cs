using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FireShotgun : MonoBehaviour
{
    public CharacterDeck Deck;
    public EntitySpawnSystem SpawnSystem;
    public UnityEvent<GameObject, GameObject> OnDischargeCard = new();

    public void PerformShotgun(GameObject target)
    {
        var shells = Deck.Hand.Where(element => element.GetComponent<Card>().SecondaryIntData > 0).ToList();
        if (!shells.Any())
            return;
        
        shells.Sort(((o, o1) => o.GetComponent<Card>().SecondaryIntData - o1.GetComponent<Card>().SecondaryIntData));
        var totalDamage =shells[0].GetComponent<Card>().SecondaryIntData;
        
        if (shells.Count >= 2)
        {
            totalDamage += shells[1].GetComponent<Card>().SecondaryIntData;
            OnDischargeCard.Invoke(shells[0], shells[1]);
        }

        else
        {
            OnDischargeCard.Invoke(shells[0], null);
        }
        
        var livingEnemies = SpawnSystem.Enemies.Where(element => !element.GetComponent<EntityHealth>().IsDead).ToList();
        if (!livingEnemies.Any())
            return;

        var dividedDamage = totalDamage / livingEnemies.Count;
        var remainder = totalDamage % livingEnemies.Count;
        livingEnemies.RandomShuffle();
        foreach (var i in Enumerable.Range(0, livingEnemies.Count))
        {
            livingEnemies[i].GetComponent<EntityHealth>().TakeDamage(dividedDamage + (i < remainder ? 1 : 0));
        }
    }
}
