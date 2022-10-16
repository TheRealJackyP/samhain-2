using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EntityDeathAnimator : MonoBehaviour
{
    public bool WaitForEvent;
    public UnityEvent<GameObject> OnEntityDeathAnimationStart = new();
    public UnityEvent<GameObject> OnEntityDeathAnimationComplete = new();
    public bool WaitingForAnimation;

    private Coroutine DoWaitForAnimationInstance;

    private void OnDestroy()
    {
        if (DoWaitForAnimationInstance != null) StopCoroutine(DoWaitForAnimationInstance);
    }

    public void EndDeathAnimation()
    {
        OnEntityDeathAnimationComplete.Invoke(gameObject);
    }

    public void DriveEntityDeathAnimation(GameObject deathTarget)
    {
        OnEntityDeathAnimationStart.Invoke(deathTarget);
        if (!WaitForEvent)
        {
            OnEntityDeathAnimationComplete.Invoke(deathTarget);
            gameObject.SetActive(false);
        }

        else
        {
            DoWaitForAnimationInstance = StartCoroutine(DoWaitForAnimation());
        }
    }

    public IEnumerator DoWaitForAnimation()
    {
        WaitingForAnimation = true;
        UnityAction<GameObject> action = _ => WaitingForAnimation = false;
        OnEntityDeathAnimationComplete.AddListener(action);
        while (WaitingForAnimation) yield return null;
        OnEntityDeathAnimationComplete.RemoveListener(action);
        DoWaitForAnimationInstance = null;
    }
}