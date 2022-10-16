using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class HoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Dragger dragger;
    public bool hovering;
    public TargetingSystem TargetingSystem;
    public int elapsedFrames;
    public int cooldownFrames;
    public int originalIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TargetingSystem ??= dragger.TargetingSystem;
        hovering = true;
        elapsedFrames = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        if (TargetingSystem.ActiveCard)
        {
            return;
        }
        StartCoroutine(StopHover());
    }

    public IEnumerator StopHover()
    {
        elapsedFrames = 0;
        while (elapsedFrames < cooldownFrames)
        {
            elapsedFrames += 1;
            yield return null;
        }
        
        dragger.StopHovering();
        TargetingSystem.ActiveTurn.GetComponent<CharacterDeck>().ReOrderHand();
    }

    private void Update()
    {
        if (hovering && !TargetingSystem.ActiveCard && Dragger.DragTarget == null && dragger.MoveToOriginalInstance == null)
        {
            dragger.StartHovering();
            originalIndex = gameObject.transform.GetSiblingIndex();
            gameObject.transform.SetAsLastSibling();
        }
    }
}