using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
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

    public Coroutine WaitForShuffleAnimationInstance;

    public bool ShuffleAnimationCompleted;
    public bool Initalized;

    public GameObject DrawPileParent;
    public GameObject HandParent;
    public GameObject DiscardPileParent;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (!Initalized)
        {
            DrawPile = DrawPile.Select(element => Instantiate(element, DrawPileParent.transform)).ToList();
            DrawPile.RandomShuffle();
            Initalized = true;
        }
    }

    public bool DrawCard()
    {
        if ((Hand.Count >= MaxHandSize) || !DrawPile.Any() && !DiscardPile.Any())
        {
            OnDrawFailed.Invoke(gameObject);
            return false;
        }

        else
        {
            if (DrawPile.Any())
            {
                var nextCard = DrawPile.First(); 
                Hand.Append(nextCard);
                DrawPile.Remove(nextCard);
                OnDrawSuccess.Invoke(nextCard);
                nextCard.transform.SetParent(HandParent.transform);
                nextCard.transform.localPosition = Vector3.zero;
                nextCard.transform.localScale = Vector3.one;
                nextCard.gameObject.SetActive(true);
                return true;
            }

            else
            {
                ShuffleDeck();
                var nextCard = DrawPile.First(); 
                Hand.Append(nextCard);
                DrawPile.Remove(nextCard);
                OnDrawSuccess.Invoke(nextCard);
                nextCard.transform.SetParent(HandParent.transform);
                nextCard.transform.localPosition = Vector3.zero;
                nextCard.transform.localScale = Vector3.one;
                nextCard.gameObject.SetActive(true);
                return true;
            }
            
        }
    }

    public void DrawCards(int number)
    {
        foreach (var _ in Enumerable.Range(0, number).ToList())
        {
            if(!DrawCard())
                break;
        }
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
        
        OnStartEndOfTurnDiscard.Invoke(gameObject);
        DiscardAll(true, false);
    }

    public void DiscardCard(GameObject card, bool endOfTurn = false, bool ignoreFailure = false)
    {
        if (!Hand.Contains(card) && !ignoreFailure)
        {
            throw new Exception("Tried to discard a card not in hand!");
        }
        
        OnDiscardCard.Invoke(card, endOfTurn);
        Hand.Remove(card);
        DiscardPile.Add(card);
        card.transform.parent = DiscardPileParent.transform;
        card.gameObject.SetActive(false);
    }

    public void DiscardCard(int cardIndex, bool endOfTurn = false, bool ignoreFailure = false)
    {
        if (cardIndex >= Hand.Count && !ignoreFailure)
        {
            throw new Exception("Tried to discard a card not in range of hand length!");
        }
        else
        {
            DiscardCard(Hand[cardIndex], endOfTurn);
        }
    }

    public void DiscardRandomCard(bool endOfTurn, bool ignoreFailure = false)
    {
        if (!Hand.Any() && !ignoreFailure)
        {
            throw new Exception("Tried to discard a card but no cards in hand!");
        }

        else
        {
            DiscardCard(Random.Range(0, Hand.Count), endOfTurn, ignoreFailure);
        }
    }

    public void DiscardMultipleCards(IEnumerable<GameObject> targetCards, bool endOfTurn = false, bool ignoreFailure = false)
    {
        targetCards.ToList().ForEach(target => DiscardCard(target, endOfTurn, ignoreFailure));
    }
    
    public void DiscardMultipleCards(IEnumerable<int> targetCardIndices, bool endOfTurn = false, bool ignoreFailure = false)
    {
        targetCardIndices.ToList().ForEach(targetIndex => DiscardCard(targetIndex, endOfTurn, ignoreFailure));
    }

    public void DiscardMultipleRandomCards(int number, bool endOfTurn = false, bool ignoreFailure = false)
    {
        Enumerable.Range(0, number).ToList().ForEach(_ => DiscardRandomCard(endOfTurn, ignoreFailure));
    }

    public void DiscardAll(bool endOfTurn = false, bool ignoreFailure = false)
    {
        Enumerable.Range(0, Hand.Count).ToList().ForEach(index => DiscardCard(index, endOfTurn, ignoreFailure));
    }

    public void ShuffleDeck()
    {
        if(WaitForShuffleAnimationInstance is not null)
            StopCoroutine(WaitForShuffleAnimationInstance);

        WaitForShuffleAnimationInstance = StartCoroutine(DoWaitForShuffleAnimation());
    }

    public IEnumerator DoWaitForShuffleAnimation()
    {
        ShuffleAnimationCompleted = false;
        OnShuffleDeck.Invoke(gameObject);
        while (!ShuffleAnimationCompleted)
        {
            yield return null;
        }
        
        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
        DrawPile.RandomShuffle();
        DrawPile.ForEach(card => ResolveShuffle(card));
    }

    public void ResolveShuffle(GameObject card)
    {
        card.transform.parent = DrawPileParent.transform;
        card.gameObject.SetActive(false);
    }

}
