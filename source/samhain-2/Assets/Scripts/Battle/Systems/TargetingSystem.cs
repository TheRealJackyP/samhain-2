using UnityEngine;
using UnityEngine.Events;

public class TargetingSystem : MonoBehaviour
{
    public GameObject ActiveTarget;
    public GameObject ActiveTurn;
    public GameObject ActiveCardAnchor;
    public GameObject ActiveCard;

    public UnityEvent<GameObject> OnTargetClicked = new();
    public UnityEvent<GameObject> OnTargetUnClicked = new();

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && ActiveTarget != null && ActiveCard != null)
        {
            var cardData = ActiveCard.GetComponent<TargetedCard>();

            if (cardData)
                if (cardData.TargetingFilter == (cardData.TargetingFilter | (1 << ActiveTarget.layer)))
                {
                    OnTargetClicked.Invoke(ActiveTarget);
                    ActiveTurn.GetComponent<Animator>().SetTrigger("Attack");
                    AudioManager.Instance.PlaySFX(SFXInterface.Instance.ShootSFX);
                }
                    
        }

        else if ((Input.GetMouseButtonDown(1) && ActiveCard != null) || (Input.GetMouseButtonDown(0) && ActiveCard != null && ActiveTarget == null))
        {
            OnTargetUnClicked.Invoke(ActiveCard);
            ActiveTurn.GetComponent<CharacterDeck>().ReOrderHand();
            ActiveCard = null;
        }
    }

    public void UpdateTurn(GameObject LastTurn, GameObject NextTurn)
    {
        ActiveTarget = null;
        ActiveCard = null;
        ActiveTurn = NextTurn;
    }
}