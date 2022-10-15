using UnityEngine;
using UnityEngine.Events;

public class TargetedCard : Card
{
    private UnityAction<GameObject> PlayTargetedCardAction;
    private UnityAction<GameObject> ReturnActiveCardAction;
    public override bool TryPlayCard(GameObject card, GameObject target, GameObject player)
    {
        var playerMana = player.GetComponent<CharacterMana>();
        var cardData = card.GetComponent<Card>();
        if (playerMana.CurrentMana < cardData.Cost)
        {
            OnFailPlayCard.Invoke(card, target, player);
            return false;
        }

        PlayTargetedCardAction = (actionTarget) => PlayTargetedCard(gameObject, actionTarget, player);
        ReturnActiveCardAction = _ => GetComponent<Dragger>().ReturnToOriginalPosition();
        OwnerDeck.TargetingSystem.OnTargetClicked.AddListener(PlayTargetedCardAction);
        OwnerDeck.TargetingSystem.OnTargetUnClicked.AddListener(ReturnActiveCardAction);
        return true;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        OwnerDeck.TargetingSystem.OnTargetClicked.RemoveListener(PlayTargetedCardAction);
        OwnerDeck.TargetingSystem.OnTargetUnClicked.RemoveListener(ReturnActiveCardAction);
    }

    public void PlayTargetedCard(GameObject card, GameObject target, GameObject player)
    {
        var playerMana = player.GetComponent<CharacterMana>();
        var cardData = card.GetComponent<Card>();
        playerMana.CurrentMana -= cardData.Cost;
        OnPlayCard.Invoke(card, target, player);
        OnStartPlayerAnimation.Invoke(card, target, player);
        OnStartCardAnimation.Invoke(card, target, player);
        OnFinishCardAnimation.Invoke(card, target, player);
        OnFinishPlayerAnimation.Invoke(card, target, player);
        OwnerDeck.TargetingSystem.OnTargetClicked.RemoveListener(PlayTargetedCardAction);
        OwnerDeck.TargetingSystem.OnTargetUnClicked.RemoveListener(ReturnActiveCardAction);
    }
}