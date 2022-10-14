using UnityEngine;
using UnityEngine.Events;

public class Card : MonoBehaviour
{
    public UnityEvent<GameObject, GameObject, GameObject> OnPlayCard = new();
    public UnityEvent<GameObject, GameObject, GameObject> OnFailPlayCard = new();

    public UnityEvent<GameObject, GameObject, GameObject> OnStartPlayerAnimation = new();
    public UnityEvent<GameObject, GameObject, GameObject> OnFinishPlayerAnimation = new();
    public UnityEvent<GameObject, GameObject, GameObject> OnStartCardAnimation = new();
    public UnityEvent<GameObject, GameObject, GameObject> OnFinishCardAnimation = new();

    public int Cost;
    public int IntData;

    public bool PlayerAnimationComplete;
    public bool CardAnimationComplete;

    private void Start()
    {
        OnFinishCardAnimation.AddListener(DoFinishPlayerAnimation);
        OnFinishPlayerAnimation.AddListener(DoFinishCardAnimation);
    }

    private void OnDestroy()
    {
        OnFinishCardAnimation.RemoveListener(DoFinishPlayerAnimation);
        OnFinishPlayerAnimation.RemoveListener(DoFinishCardAnimation);
    }

    public virtual void TryPlayCard(GameObject card, GameObject target, GameObject player)
    {
        var playerMana = player.GetComponent<CharacterMana>();
        var cardData = card.GetComponent<Card>();
        if (playerMana.CurrentMana < cardData.Cost)
        {
            OnFailPlayCard.Invoke(card, target, player);
            return;
        }

        playerMana.CurrentMana -= cardData.Cost;
        OnPlayCard.Invoke(card, target, player);
        OnStartPlayerAnimation.Invoke(card, target, player);
        OnStartCardAnimation.Invoke(card, target, player);
        OnFinishCardAnimation.Invoke(card, target, player);
        OnFinishPlayerAnimation.Invoke(card, target, player);
    }

    public virtual void DoFinishPlayerAnimation(GameObject card, GameObject target, GameObject player)
    {
        PlayerAnimationComplete = true;
        FinishPlayingCard(card, target, player);
    }

    public virtual void DoFinishCardAnimation(GameObject card, GameObject target, GameObject player)
    {
        CardAnimationComplete = true;
        FinishPlayingCard(card, target, player);
    }

    public virtual void FinishPlayingCard(GameObject card, GameObject target, GameObject player)
    {
        if (PlayerAnimationComplete && CardAnimationComplete)
            Destroy(gameObject);
    }
}