using UnityEngine;

public class UpdateArmor : MonoBehaviour
{
    public void AddArmor(GameObject card, GameObject target, GameObject player)
    {
        target.GetComponent<EntityHealth>().Armor += card.GetComponent<Card>().IntData;
    }
}