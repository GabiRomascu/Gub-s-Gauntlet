using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, 5f); // Destroy the bullet after 5 seconds

        // Ignore collision with the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D bulletCollider = GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            BossHP bossScript = collision.gameObject.GetComponent<BossHP>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            if (bossScript != null)
            {
                bossScript.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy the bullet on collision with enemy or boss
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy the bullet on collision with wall
        }
    }
}
