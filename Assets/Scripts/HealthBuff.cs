using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/HealthBuff")]
public class HealthBuff : PowerUP
{
    public float amount;

    public override void Apply(GameObject target)
    {
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.currentHealth += amount;
            if (playerHealth.currentHealth > playerHealth.maxHealth)
            {
                playerHealth.currentHealth = playerHealth.maxHealth;
            }
            playerHealth.UpdateHealthBar();
        }
        else
        {
            Debug.LogWarning("No PlayerHealth component found on target.");
        }
    }
}
