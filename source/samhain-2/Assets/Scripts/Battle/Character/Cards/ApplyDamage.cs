using UnityEngine;

public class ApplyDamage : MonoBehaviour
{
    public void ApplyCardDamage(GameObject card, GameObject target, GameObject player)
    {
        target.GetComponent<EntityHealth>().TakeDamage(card.GetComponent<Card>().IntData);
    }
}