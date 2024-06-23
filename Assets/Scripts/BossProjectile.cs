using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, 5f); // Destroy the bullet after 5 seconds

        // Ignore collision with enemies
        Collider2D bulletCollider = GetComponent<Collider2D>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, bulletCollider);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy the bullet on collision with wall
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy the bullet on collision with player
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy the bullet on collision with wall
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy the bullet on collision with player
        }
    }
}
