using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnchor : MonoBehaviour
{
    public HandAnchorPositioningSystem PositioningSystem;
    public Card AnchorCard;

    public void ActivateAnchor(GameObject card)
    {
        AnchorCard = card.GetComponent<Card>();
        AnchorCard.GetComponent<Dragger>().HandAnchor = this;
        gameObject.SetActive(true);
    }

    public void DeactivateAnchor()
    {
        AnchorCard = null;
        gameObject.SetActive(false);
    }
}
