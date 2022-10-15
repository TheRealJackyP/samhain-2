using UnityEngine;

public class EntityTargeting : MonoBehaviour
{
    public TargetingSystem TargetingSystem;

    private void OnMouseEnter()
    {
        TargetingSystem.ActiveTarget = gameObject;
    }

    private void OnMouseExit()
    {
        TargetingSystem.ActiveTarget = null;
    }
}