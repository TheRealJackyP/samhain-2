using System;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    public int BaseHealth = 100;
    public int _currentHealth = 100;
    public int _armor;
    public string EntityName;

    public UnityEvent<GameObject> OnEntityDeath = new();
    public UnityEvent<GameObject> OnArmorBreak = new();
    public UnityEvent<GameObject> OnArmorGain = new();

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            if (_currentHealth <= 0)
                OnEntityDeath.Invoke(gameObject);
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
    }

    public void ResetHealth()
    {
        CurrentHealth = BaseHealth;
    }
}