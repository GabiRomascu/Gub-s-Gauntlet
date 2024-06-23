using UnityEngine;

public class LootPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LootBehavior behavior = GetComponent<LootBehavior>();
            if (behavior != null)
            {
                behavior.OnPickup(collision.gameObject);
            }
        }
    }
}
