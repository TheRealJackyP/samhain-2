using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Dragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static readonly float DropTimeout = 2f;
    public TargetingSystem TargetingSystem;
    public bool dragOnSurfaces = true;
    public Vector3 OriginalPosition;
    public float LerpSpeed;

    private RectTransform m_DraggingPlane;

    private Coroutine MoveToOriginalInstance;

    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null || MoveToOriginalInstance is not null)
            return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.

        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;

        OriginalPosition = GetComponent<RectTransform>().position;
        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData data)
    {
        SetDraggedPosition(data);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var card = GetComponent<Card>();
        if (card is UntargetedCard)
        {
            if (!card.TryPlayCard(card.gameObject, TargetingSystem.ActiveTurn, TargetingSystem.ActiveTurn))
            {
                if (MoveToOriginalInstance is not null)
                    StopCoroutine(MoveToOriginalInstance);

                MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(OriginalPosition));
            }

            return;
        }


        if (card is TargetedCard targetedCard && TargetingSystem.ActiveTarget != null)
        {
            if (!targetedCard.TryPlayCard(card.gameObject, TargetingSystem.ActiveTarget, TargetingSystem.ActiveTurn))
            {
                if (MoveToOriginalInstance is not null)
                    StopCoroutine(MoveToOriginalInstance);

                MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(OriginalPosition));
            }

            return;
        }

        if (MoveToOriginalInstance is not null)
            StopCoroutine(MoveToOriginalInstance);

        MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(OriginalPosition));
    }

    public IEnumerator LerpToTargetPosition(Vector3 targetPosition)
    {
        var rect = GetComponent<RectTransform>();
        var elapsedTime = 0f;
        while (Vector3.Distance(rect.transform.position, targetPosition) > .01f && elapsedTime < DropTimeout)
        {
            rect.transform.position =
                Vector3.MoveTowards(rect.transform.position, targetPosition, LerpSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rect.transform.position = targetPosition;
        MoveToOriginalInstance = null;
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position,
                data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

    public static T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        var t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }

        return comp;
    }
}