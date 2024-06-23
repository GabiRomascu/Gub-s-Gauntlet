using UnityEngine;
using UnityEngine.UI;

public class BossHP : MonoBehaviour
{
    public int maxHealth = 200;
    public int currentHealth;

    public Image healthBar;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        UpdateHealthBar();
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        // Ensure healthBar is not null to avoid errors
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        // Handle enemy death
        Destroy(gameObject);
    }
}
