using TMPro;
using UnityEngine;

public class ManaCounterUI : MonoBehaviour
{
    public TurnSystem TurnSystem;
    public TMP_Text Text;

    // Update is called once per frame
    private void Update()
    {
        if (TurnSystem.GetCurrentTurn().TryGetComponent<CharacterMana>(out var ActiveMana))
            Text.text = ActiveMana.CurrentMana + "/" + ActiveMana.MaxMana;
    }
}