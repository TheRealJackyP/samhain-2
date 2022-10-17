using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class CharacterDeck : MonoBehaviour
{
    public List<GameObject> DrawPile = new();
    public List<GameObject> Hand = new();
    public List<GameObject> DiscardPile = new();

    public int CardDrawStartofTurn;
    public int MaxHandSize;

    public UnityEvent<GameObject> OnDrawFailed = new();
    public UnityEvent<GameObject> OnDrawSuccess = new();
    public UnityEvent<GameObject> OnShuffleDeck = new();
    public UnityEvent<GameObject> OnShuffleDeckAnimationComplete = new();
    public UnityEvent<GameObject, bool> OnDiscardCard = new();
    public UnityEvent<GameObject> OnStartEndOfTurnDiscard = new();

    public bool ShuffleAnimationCompleted;
    public bool Initalized;

    public GameObject DrawPileParent;
    public GameObject HandParent;
    public GameObject DiscardPileParent;

    public TargetingSystem TargetingSystem;

    public int CardsToRetry;

    public bool WaitForShuffleAnimation;

    public Coroutine WaitForShuffleAnimationInstance;

    private void Start()
    {
        if (DrawPile.Any())
            Initialize();
    }

    private void OnDestroy()
    {
        OnDrawFailed.RemoveAllListeners();
        OnDrawSuccess.RemoveAllListeners();
        OnShuffleDeck.RemoveAllListeners();
        OnShuffleDeckAnimationComplete.RemoveAllListeners();
        OnDiscardCard.RemoveAllListeners();
        OnStartEndOfTurnDiscard.RemoveAllListeners();
    }

    public void Initialize()
    {
        if (!Initalized)
        {
            DrawPile = DrawPile.Select(element => Instantiate(element, DrawPileParent.transform)).ToList();
            DrawPile.RandomShuffle();
            DrawPile.ForEach(element => element.GetComponent<Dragger>().TargetingSystem = TargetingSystem);
            DrawPile.ForEach(element => element.GetComponent<Card>().OwnerDeck = this);
            Initalized = true;
        }
    }

    public void ReOrderHand()
    {
        Hand.Sort((anchor, handAnchor) =>
            Mathf.RoundToInt(anchor.transform.localPosition.x - handAnchor.transform.localPosition.x));
        foreach (var i in Enumerable.Range(0, Hand.Count)) Hand[i].transform.SetAsLastSibling();
    }

    public bool DrawCard()
    {
        if (Hand.Count >= MaxHandSize || (!DrawPile.Any() && !DiscardPile.Any()))
        {
            OnDrawFailed.Invoke(gameObject);
            return false;
        }

        if (DrawPile.Any())
        {
            var nextCard = DrawPile.First();
            Hand.Add(nextCard);
            DrawPile.Remove(nextCard);
            OnDrawSuccess.Invoke(nextCard);
            nextCard.transform.SetParent(HandParent.transform);
            nextCard.transform.localPosition = Vector3.zero;
            nextCard.transform.localScale = Vector3.one;
            nextCard.gameObject.SetActive(true);
            AudioManager.Instance.PlaySFX(SFXInterface.Instance.DrawSFX);
            if (nextCard.TryGetComponent<TargetedCard>(out var card)) card.ResetShellImage();
            return true;
        }

        // ShuffleDeck();
        return false;
    }

    public void DrawCards(int number)
    {
        var remaining = number;
        if (remaining <= DrawPile.Count)
        {
            foreach (var _ in Enumerable.Range(0, remaining).ToList())
                if (!DrawCard())
                    throw new Exception("Did some bad math calculating draw amounts!");

            return;
        }

        remaining -= DrawPile.Count;
        foreach (var _ in Enumerable.Range(0, DrawPile.Count))
            if (!DrawCard())
                throw new Exception("Did some bad math calculating draw amounts!");

        if (remaining > DiscardPile.Count)
        {
            ShuffleDeck(DiscardPile.Count);
            return;
        }

        ShuffleDeck(remaining);
    }

    public void DrawCardsStartTurn(GameObject pastTurn, GameObject currentTurn)
    {
        if (currentTurn == gameObject)
        {
            Initialize();
            DrawCards(CardDrawStartofTurn);
        }
    }

    public void DiscardEndOfTurn(GameObject endingTurn)
    {
        if (endingTurn != gameObject)
            return;

        if (endingTurn.GetComponent<EntityHealth>().EntityName == "Hermes")
        {
            StartCoroutine(WaitToDiscard());
        }
        else
        {
            AudioManager.Instance.PlaySFX(SFXInterface.Instance.DiscardSFX);
            OnStartEndOfTurnDiscard.Invoke(gameObject);
            DiscardAll(true);
        }
    }

    public IEnumerator WaitToDiscard()
    {
        var elapsedTime = 0f;

        var shells = Hand.Where(element => element.GetComponent<Card>().SecondaryIntData > 0).ToList();
        if (shells.Any())
        {
            shells.Sort((o, o1) => o.GetComponent<Card>().SecondaryIntData - o1.GetComponent<Card>().SecondaryIntData);

            if (shells.Count >= 2)
            {
                shells[0].GetComponent<TargetedCard>().MoveToGun(null);
                shells[1].GetComponent<TargetedCard>().MoveToGun(null);
            }

            else
            {
                shells[0].GetComponent<TargetedCard>().MoveToGun(null);
            }

            while (elapsedTime <= 1.1f)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            elapsedTime = 0;
            GetComponent<Animator>().SetTrigger("Shotgun");
            AudioManager.Instance.PlaySFX(SFXInterface.Instance.ShootSFX);

            // while (elapsedTime <= 1.5f)
            // {
            //     yield return null;
            //     elapsedTime += Time.deltaTime;
            // }
        }
        else
        {
            yield return null;
        }


        // while (elapsedTime <= 1.5f)
        // {
        //     yield return null;
        //     elapsedTime += Time.deltaTime;
        // }

        AudioManager.Instance.PlaySFX(SFXInterface.Instance.DiscardSFX);
        OnStartEndOfTurnDiscard.Invoke(gameObject);
        GetComponent<FireShotgun>().SpawnSystem.TurnSystem.OnTurnStartAnimationComplete.Invoke();
        DiscardAll(true);
    }

    public void DiscardCard(GameObject card, bool endOfTurn = false, bool ignoreFailure = false)
    {
        if (!Hand.Contains(card) && !ignoreFailure) throw new Exception("Tried to discard a card not in hand!");

        var cardData = card.GetComponent<Card>();
        cardData.CardAnimationComplete = false;
        cardData.PlayerAnimationComplete = false;

        OnDiscardCard.Invoke(card, endOfTurn);
        Hand.Remove(card);
        DiscardPile.Add(card);
        card.transform.SetParent(DiscardPileParent.transform);
        card.gameObject.SetActive(false);
    }

    public void DiscardCard(int cardIndex, bool endOfTurn = false, bool ignoreFailure = false)
    {
        if (cardIndex >= Hand.Count && !ignoreFailure)
            throw new Exception("Tried to discard a card not in range of hand length!");
        DiscardCard(Hand[cardIndex], endOfTurn);
    }

    public void DiscardRandomCard(bool endOfTurn, bool ignoreFailure = false)
    {
        if (!Hand.Any() && !ignoreFailure)
            throw new Exception("Tried to discard a card but no cards in hand!");

        DiscardCard(Random.Range(0, Hand.Count), endOfTurn, ignoreFailure);
    }

    public void DiscardMultipleCards(IEnumerable<GameObject> targetCards, bool endOfTurn = false,
        bool ignoreFailure = false)
    {
        targetCards.ToList().ForEach(target => DiscardCard(target, endOfTurn, ignoreFailure));
    }

    // public void DiscardMultipleCards(IEnumerable<int> targetCardIndices, bool endOfTurn = false, bool ignoreFailure = false)
    // {
    //     targetCardIndices.ToList().ForEach(targetIndex => DiscardCard(targetIndex, endOfTurn, ignoreFailure));
    // }

    public void DiscardMultipleRandomCards(int number, bool endOfTurn = false, bool ignoreFailure = false)
    {
        Enumerable.Range(0, number).ToList().ForEach(_ => DiscardRandomCard(endOfTurn, ignoreFailure));
    }

    public void DiscardAll(bool endOfTurn = false, bool ignoreFailure = false)
    {
        Enumerable.Range(0, Hand.Count).ToList().ForEach(index => DiscardCard(0, endOfTurn, ignoreFailure));
    }

    public void ShuffleDeck(int number = 0)
    {
        if (WaitForShuffleAnimationInstance is not null)
            StopCoroutine(WaitForShuffleAnimationInstance);

        WaitForShuffleAnimationInstance = StartCoroutine(DoWaitForShuffleAnimation(number));
    }

    public IEnumerator DoWaitForShuffleAnimation(int number = 0)
    {
        ShuffleAnimationCompleted = false;
        OnShuffleDeck.Invoke(gameObject);
        while (!ShuffleAnimationCompleted && WaitForShuffleAnimation) yield return null;

        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
        DrawPile.RandomShuffle();
        DrawPile.ForEach(card => ResolveShuffle(card));
        DrawCards(number);
    }

    public void ResolveShuffle(GameObject card)
    {
        card.transform.SetParent(DrawPileParent.transform);
        card.gameObject.SetActive(false);
    }
}