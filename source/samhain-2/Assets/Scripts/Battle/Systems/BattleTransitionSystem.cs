using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleTransitionSystem : MonoBehaviour
{
    public UnityEvent OnStartBattle = new();
    public UnityEvent OnStartBattleAnimationComplete = new();
    public UnityEvent<bool> OnEndBattle = new();
    public UnityEvent<bool> OnEndBattleAnimationComplete = new();
    public EntitySpawnSystem EntitySpawnSystem;
    public bool triggered;

    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;

    public void Update()
    {
        if (EntitySpawnSystem.Characters.All(element => element.GetComponent<EntityHealth>().IsDead) && !triggered)
        {
            EntitySpawnSystem.Characters.ForEach(element =>
                EntitySpawnSystem.BattleDirectives.CharacterHealth[element.GetComponent<EntityHealth>().EntityName] =
                    Math.Clamp(element.GetComponent<EntityHealth>().CurrentHealth, 1,
                        element.GetComponent<EntityHealth>().BaseHealth));
            triggered = true;
            AudioManager.Instance.PlayMusic(SFXInterface.Instance.GameMusic);
            OnEndBattle.Invoke(false);
        }


        else if (EntitySpawnSystem.Enemies.All(element => element.GetComponent<EntityHealth>().IsDead)&& !triggered)
        {
            EntitySpawnSystem.Characters.ForEach(element =>
                EntitySpawnSystem.BattleDirectives.CharacterHealth[element.GetComponent<EntityHealth>().EntityName] =
                    Math.Clamp(element.GetComponent<EntityHealth>().CurrentHealth, 1,
                        element.GetComponent<EntityHealth>().BaseHealth));
            triggered = true;
            AudioManager.Instance.PlayMusic(SFXInterface.Instance.GameMusic);
            OnEndBattle.Invoke(true);
        }
    }

    public void CheckForBattleEnd(GameObject deadTarget)
    {
        // if (EntitySpawnSystem.Characters.Contains(deadTarget) && EntitySpawnSystem.Characters.Count(element => element.GetComponent<EntityHealth>().IsDead || !element.activeSelf) + 1 == EntitySpawnSystem.Characters.Count)
        // {
        //     OnEndBattle.Invoke(false);
        // }
        //
        // if (EntitySpawnSystem.Enemies.Contains(deadTarget) && EntitySpawnSystem.Enemies.Count(element => element.GetComponent<EntityHealth>().IsDead|| !element.activeSelf) + 1 == EntitySpawnSystem.Characters.Count)
        // {
        //     OnEndBattle.Invoke(true);
        // }
    }
}