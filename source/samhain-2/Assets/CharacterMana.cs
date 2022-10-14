using System;
using UnityEngine;

public class CharacterMana : MonoBehaviour
{
    public int CurrentMana;
    public int MaxMana;

    private void Start()
    {
        RefillMana();
    }

    public void RefillMana()
    {
        CurrentMana = MaxMana;
    }

    public void AddMana(int amount, bool overfill = false)
    {
        CurrentMana += amount;
        if (!overfill)
            CurrentMana = Math.Clamp(CurrentMana, 0, MaxMana);
    }
}