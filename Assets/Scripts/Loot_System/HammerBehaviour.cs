using UnityEngine;

public class HammerBehavior : LootBehavior
{
    public float buffDuration = 5f; // Duration of the buff in seconds

    public override void OnPickup(GameObject player)
    {
        PlayerShooting playerShooting = player.GetComponent<PlayerShooting>();
        if (playerShooting != null)
        {
            playerShooting.DoubleBulletDamage(buffDuration);
        }
        Destroy(gameObject);
    }
}
