using UnityEngine;

public abstract class LootBehavior : MonoBehaviour
{
    public abstract void OnPickup(GameObject player);
}
