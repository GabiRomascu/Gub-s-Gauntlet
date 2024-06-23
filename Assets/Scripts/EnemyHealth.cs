using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public event Action onDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

     void Die()
    {
        // Loot
        LootBag lootBag = GetComponent<LootBag>();
        if (lootBag != null)
        {
            lootBag.InstantiateLoot(transform.position);
        }
        else
        {
            Debug.LogWarning("LootBag component not found on " + gameObject.name);
        }

        // Handle enemy death
        Destroy(gameObject);
    }
}
