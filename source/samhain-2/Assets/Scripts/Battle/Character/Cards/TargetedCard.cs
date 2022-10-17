using UnityEngine;
using UnityEngine.Events;

public class TargetedCard : Card
{
    public LayerMask TargetingFilter;
    public GameObject ShellImage;
    private UnityAction<GameObject> PlayTargetedCardAction;
    private UnityAction<GameObject> ReturnActiveCardAction;

    private void Update()
    {
        if (OwnerDeck.GetComponent<EntityHealth>().EntityName != "Hermes")
        {
            ShellImage.SetActive(false);
            GetComponent<Animator>().SetBool("Off", true);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (PlayTargetedCardAction != null)
            OwnerDeck.TargetingSystem.OnTargetClicked.RemoveListener(PlayTargetedCardAction);

        if (ReturnActiveCardAction != null)
            OwnerDeck.TargetingSystem.OnTargetUnClicked.RemoveListener(ReturnActiveCardAction);
    }

    public void ResetShellImage()
    {
        if (OwnerDeck.GetComponent<EntityHealth>().EntityName != "Hermes")
        {
            ShellImage.SetActive(true);
            GetComponent<Animator>().SetTrigger("Reset");
        }
    }

    public void MoveToGun(GameObject target)
    {
        if (OwnerDeck.GetComponent<EntityHealth>().EntityName == "Hermes") GetComponent<Animator>().SetTrigger("Move");
    }


    public override bool TryPlayCard(GameObject card, GameObject target, GameObject player)
    {
        var playerMana = player.GetComponent<CharacterMana>();
        var cardData = card.GetComponent<Card>();
        if (playerMana.CurrentMana < cardData.Cost)
        {
            OnFailPlayCard.Invoke(card, target, player);
            return false;
        }

        PlayTargetedCardAction = actionTarget => PlayTargetedCard(gameObject, actionTarget, player);
        ReturnActiveCardAction = actionCard => ReturnTargetedCard(actionCard, target, player);
        OwnerDeck.TargetingSystem.OnTargetClicked.AddListener(PlayTargetedCardAction);
        OwnerDeck.TargetingSystem.OnTargetUnClicked.AddListener(ReturnActiveCardAction);
        OwnerDeck.TargetingSystem.ActiveCard = gameObject;
        return true;
    }

    public void PlayTargetedCard(GameObject card, GameObject target, GameObject player)
    {
        var playerMana = player.GetComponent<CharacterMana>();
        var cardData = card.GetComponent<Card>();
        playerMana.CurrentMana -= cardData.Cost;
        Dragger.DragTarget = null;
        OnPlayCard.Invoke(card, target, player);
        OnStartPlayerAnimation.Invoke(card, target, player);
        OnStartCardAnimation.Invoke(card, target, player);
        OnFinishCardAnimation.Invoke(card, target, player);
        OnFinishPlayerAnimation.Invoke(card, target, player);
        OwnerDeck.TargetingSystem.OnTargetClicked.RemoveListener(PlayTargetedCardAction);
        OwnerDeck.TargetingSystem.OnTargetUnClicked.RemoveListener(ReturnActiveCardAction);
        PlayTargetedCardAction = null;
        ReturnActiveCardAction = null;
    }

    public void ReturnTargetedCard(GameObject card, GameObject target, GameObject player)
    {
        GetComponent<Dragger>().ReturnToOriginalPosition();
        Dragger.DragTarget = null;
        OwnerDeck.TargetingSystem.OnTargetClicked.RemoveListener(PlayTargetedCardAction);
        OwnerDeck.TargetingSystem.OnTargetUnClicked.RemoveListener(ReturnActiveCardAction);
        PlayTargetedCardAction = null;
        ReturnActiveCardAction = null;
    }
}