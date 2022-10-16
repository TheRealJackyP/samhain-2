using System;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    public int BaseHealth = 100;
    public int _currentHealth = 100;
    public int _armor;
    public string EntityName;
    public bool IsDead;

    public UnityEvent<GameObject> OnEntityDeath = new();
    public UnityEvent<GameObject> OnArmorBreak = new();
    public UnityEvent<GameObject> OnArmorGain = new();
    public UnityEvent<GameObject> OnTakeDamage = new();

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Math.Clamp(value, 0, BaseHealth);
            if (_currentHealth <= 0)
            {
                OnEntityDeath.Invoke(gameObject);
                IsDead = true;
            }
        }
    }

    public int Armor
    {
        get => _armor;
        set
        {
            if (_armor == 0 && value != 0)
                OnArmorGain.Invoke(gameObject);
            _armor = value;
            if (_armor <= 0)
                OnArmorBreak.Invoke(gameObject);
        }
    }

    private void OnDestroy()
    {
        OnEntityDeath.RemoveAllListeners();
        OnArmorBreak.RemoveAllListeners();
        OnArmorGain.RemoveAllListeners();
    }

    // public void Start()
    // {
    //     ResetHealth();
    // }

    public void TakeDamage(int amount)
    {
        var unblocked = amount;
        if (amount > Armor && Armor > 0)
        {
            unblocked -= Armor;
            Armor = 0;
            CurrentHealth = Math.Clamp(CurrentHealth - unblocked, 0, BaseHealth);
        }
        else if (Armor > 0)
        {
            Armor = Math.Clamp(Armor - unblocked, 0, Armor);
        }

        else
        {
            CurrentHealth = Math.Clamp(CurrentHealth - unblocked, 0, BaseHealth);
        }
        
        if(amount > 0)
            OnTakeDamage.Invoke(gameObject);
    }

    public void ResetHealth()
    {
        CurrentHealth = BaseHealth;
    }
}