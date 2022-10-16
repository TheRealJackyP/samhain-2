using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Dragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject DragTarget;
    private static readonly float DropTimeout = 2f;
    public TargetingSystem TargetingSystem;
    public bool dragOnSurfaces = true;
    public float LerpSpeed;
    public float RotateSpeed;
    [SerializeField] private HandAnchor _handAnchor;
    public Vector3 HoverOffset = Vector3.zero;

    private RectTransform m_DraggingPlane;

    private Coroutine MoveToOriginalInstance;

    public HandAnchor HandAnchor
    {
        get => _handAnchor;
        set
        {
            if (_handAnchor == null && value != null)
            {
                _handAnchor = value;
                Init();
                return;
            }

            _handAnchor = value;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (DragTarget != null && DragTarget != gameObject)
            return;
        DragTarget = gameObject;
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null || MoveToOriginalInstance is not null)
            return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.

        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;

        if(MoveToOriginalInstance == null)
            SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData data)
    {
        if (DragTarget != null && DragTarget != gameObject)
            return;
        if (Input.GetMouseButtonDown(1))
        {
            ReturnToOriginalPosition(true);
            DragTarget = null;
            return;
        }

        if(MoveToOriginalInstance == null)
            SetDraggedPosition(data);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DragTarget != null && DragTarget != gameObject)
            return;

        var card = GetComponent<Card>();
        if (card is UntargetedCard)
        {
            if (GetComponent<RectTransform>().transform.localPosition.y <= 300f || !card.TryPlayCard(card.gameObject, TargetingSystem.ActiveTurn, TargetingSystem.ActiveTurn))
            {
                if (MoveToOriginalInstance is not null)
                    StopCoroutine(MoveToOriginalInstance);

                MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(HandAnchor.GetComponent<RectTransform>()));
            }

            DragTarget = null;
            return;
        }


        if (card is TargetedCard targetedCard)
        {
            if (GetComponent<RectTransform>().transform.localPosition.y <= 300f || !targetedCard.TryPlayCard(card.gameObject, TargetingSystem.ActiveTarget, TargetingSystem.ActiveTurn) )
            {
                if (MoveToOriginalInstance is not null)
                    StopCoroutine(MoveToOriginalInstance);

                MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(HandAnchor.GetComponent<RectTransform>()));
                DragTarget = null;
                return;
            }

            if (MoveToOriginalInstance is not null)
                StopCoroutine(MoveToOriginalInstance);

            MoveToOriginalInstance =
                StartCoroutine(LerpToTargetPosition(TargetingSystem.ActiveCardAnchor.GetComponent<RectTransform>()));
            return;
        }

        if (MoveToOriginalInstance is not null)
            StopCoroutine(MoveToOriginalInstance);

        MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(HandAnchor.GetComponent<RectTransform>()));
        DragTarget = null;
    }

    public void Init()
    {
        DragTarget = null;
        if (MoveToOriginalInstance is not null)
            StopCoroutine(MoveToOriginalInstance);

        MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(HandAnchor.GetComponent<RectTransform>()));
    }

    public void ReturnToOriginalPosition(bool stopHover = false)
    {
        if (MoveToOriginalInstance is not null)
            StopCoroutine(MoveToOriginalInstance);

        MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(HandAnchor.GetComponent<RectTransform>(), false, stopHover));
        DragTarget = null;
    }

    public void StartHovering()
    {
        if (MoveToOriginalInstance is not null)
            StopCoroutine(MoveToOriginalInstance);

        MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(HandAnchor.GetComponent<RectTransform>(), true));
    }

    public void StopHovering()
    {
        if (MoveToOriginalInstance is not null)
            StopCoroutine(MoveToOriginalInstance);

        MoveToOriginalInstance = StartCoroutine(LerpToTargetPosition(HandAnchor.GetComponent<RectTransform>(), false));
    }

    public IEnumerator LerpToTargetPosition(RectTransform target, bool hover = false, bool disableHover= false)
    {
        var rect = GetComponent<RectTransform>();
        var elapsedTime = 0f;

        if (disableHover)
            GetComponent<HoverDetector>().enabled = false;

        var transformPosition = target.position + (hover ? HoverOffset : Vector3.zero);
        while ((Vector3.Distance(rect.transform.position, transformPosition) > .01f ||
                Mathf.Abs(Quaternion.Angle(target.rotation, rect.rotation)) > .01f) && elapsedTime < DropTimeout)
        {
            rect.transform.position =
                Vector3.MoveTowards(rect.transform.position, transformPosition, LerpSpeed * Time.deltaTime * (hover ? .5f : 1));

            rect.rotation = Quaternion.RotateTowards(rect.rotation, target.rotation, RotateSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rect.transform.position = transformPosition;
        rect.rotation = target.rotation;
        if (disableHover)
            GetComponent<HoverDetector>().enabled = true;
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