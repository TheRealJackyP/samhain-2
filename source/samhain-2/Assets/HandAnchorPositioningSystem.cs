using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HandAnchorPositioningSystem : MonoBehaviour
{
    public List<HandAnchor> InActiveAnchors = new();
    public List<HandAnchor> ActiveAnchors = new();
    
    public int NumActiveCards;
    public float Angle;
    public float Displacement;
    public EntitySpawnSystem SpawnSystem;

    public UnityAction<GameObject> DrawAction;
    public UnityAction<GameObject, bool> DiscardAction;
    public GameObject OverrideCanvas;

    public void Initialize()
    {
        SpawnSystem.Characters.ForEach(SubscribeToEvents);
    }

    private void OnDestroy()
    {
        // SpawnSystem.Characters.ForEach(UnsubscribeFromEvents);
    }

    public void SubscribeToEvents(GameObject character)
    {
        var deck = character.GetComponent<CharacterDeck>();
        DrawAction = UpdateAnchorsOnDraw;
        DiscardAction = (card, end) => UpdateAnchorsOnPlay(card, null, null);
        deck.OnDrawSuccess.AddListener(DrawAction);
        deck.OnDiscardCard.AddListener(DiscardAction);
    }

    public void UnsubscribeFromEvents(GameObject character)
    {
        var deck = character.GetComponent<CharacterDeck>();
        deck.OnDrawSuccess.RemoveListener(DrawAction);
        deck.OnDiscardCard.RemoveListener(DiscardAction);
    }
    
    public void UpdateAnchorsOnPlay(GameObject card, GameObject target, GameObject player)
    {
        if (card != null)
        {
            var targetAnchor = ActiveAnchors.First(element => element.AnchorCard.gameObject == card.gameObject);
            targetAnchor.DeactivateAnchor();
            InActiveAnchors.Add(targetAnchor);
            ActiveAnchors.Remove(targetAnchor);
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        
        yield return null;
        DetermineAnchorRotation();
        ActiveAnchors.ForEach(element=> element.AnchorCard.GetComponent<Dragger>().Init());
        if(SpawnSystem.TargetingSystem.ActiveTurn.TryGetComponent<CharacterDeck>(out var deck))
            deck.ReOrderHand();
    }

    public void DetermineAnchorRotation()
    {
        if ((ActiveAnchors.Count % 2) == 0)
        {
            var sortedAnchors = ActiveAnchors.ToList();
            sortedAnchors.Sort(((anchor, handAnchor) => Mathf.RoundToInt(anchor.transform.localPosition.x - handAnchor.transform.localPosition.x)));
            var multiplier = -ActiveAnchors.Count / 2;
            foreach (var i in Enumerable.Range(0, ActiveAnchors.Count))
            {
                sortedAnchors[i].transform.rotation = Quaternion.Euler(multiplier * Angle *.8f * Vector3.back);
                // sortedAnchors[i].transform.localPosition = Mathf.Abs(multiplier) * Displacement * Vector3.down;
                multiplier += 1;
                if (multiplier == 0)
                    multiplier += 1;
            }
        }

        else
        {
            var sortedAnchors = ActiveAnchors.ToList();
            sortedAnchors.Sort(((anchor, handAnchor) => Mathf.RoundToInt(anchor.transform.localPosition.x - handAnchor.transform.localPosition.x)));
            var multiplier = -((ActiveAnchors.Count / 2));
            foreach (var i in Enumerable.Range(0, ActiveAnchors.Count))
            {
                sortedAnchors[i].transform.rotation = Quaternion.Euler(multiplier * Angle * Vector3.back);
                // sortedAnchors[i].transform.localPosition = Mathf.Abs(multiplier) * Displacement * Vector3.down;
                multiplier += 1;
            }
        }
    }

    public void UpdateAnchorsOnDraw(GameObject card)
    {
        var targetAnchor = InActiveAnchors.First();
        InActiveAnchors.Remove(targetAnchor);
        ActiveAnchors.Add(targetAnchor);
        targetAnchor.ActivateAnchor(card);
        StartCoroutine(Wait());
    }

}
