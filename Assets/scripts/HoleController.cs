using UnityEngine;

public class HoleController : MonoBehaviour
{
    public delegate void OnPlyerFinish();
    public static event OnPlyerFinish onPlayerFinish;
    void OnTriggerEnter(Collider col)
    {
        // trigger the event if a player enters the hole
        onPlayerFinish();
    }
}
