using UnityEngine;

public class RestingPoint : MonoBehaviour, IRaycastable
{
    public void OnHitByRaycast()
    {
        TimeEvents.newDay.Invoke();
        Debug.Log("Mimimimmimimmimimi");
    }
}
