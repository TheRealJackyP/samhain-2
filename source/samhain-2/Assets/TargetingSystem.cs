using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public GameObject ActiveTarget;
    public GameObject ActiveTurn;

    public void UpdateTurn(GameObject LastTurn, GameObject NextTurn)
    {
        ActiveTarget = null;
        ActiveTurn = NextTurn;
    }
}