using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCounterUI : MonoBehaviour
{
    public TurnSystem TurnSystem;
    public TMPro.TMP_Text Text;

    // Update is called once per frame
    void Update()
    {
        if (TurnSystem.GetCurrentTurn().TryGetComponent<CharacterMana>(out var ActiveMana))
        {
            Text.text = ActiveMana.CurrentMana + "/" + ActiveMana.MaxMana;
        }
    }
}
