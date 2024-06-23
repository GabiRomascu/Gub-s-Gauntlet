using UnityEngine;

public class SpeedBoostBehavior : LootBehavior
{
    public float speedMultiplier = 1.2f;
    public float duration = 6f;

    public override void OnPickup(GameObject player)
    {
        CooldownManager cooldownManager = FindObjectOfType<CooldownManager>();
        if (cooldownManager != null && !cooldownManager.IsAmuletOnCooldown())
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.StartCoroutine(playerMovement.ApplySpeedBoost(speedMultiplier, duration));
            }

            cooldownManager.StartAmuletDuration();
            Destroy(gameObject);
        }
    }


}
