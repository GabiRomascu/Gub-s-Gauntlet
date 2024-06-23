using UnityEngine;

public class PotionBehavior : LootBehavior
{
    public float healAmount = 20f;

    public override void OnPickup(GameObject player)
    {
        CooldownManager cooldownManager = FindObjectOfType<CooldownManager>();
        if (cooldownManager != null && !cooldownManager.IsPotionOnCooldown())
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.currentHealth += healAmount;
                if (playerHealth.currentHealth > playerHealth.maxHealth)
                {
                    playerHealth.currentHealth = playerHealth.maxHealth;
                }
                playerHealth.UpdateHealthBar();
            }

            cooldownManager.StartPotionDuration();
            Destroy(gameObject);
        }
    }
}
