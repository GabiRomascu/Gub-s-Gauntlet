using UnityEngine;

public class SingleShootingEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 2f; // Speed of the bullet
    public float shootingInterval = 2f;
    public float followRange = 5f; // Range within which the enemy follows the player
    public float shootingRange = 5f; // Range within which the enemy can shoot the player
    private float shootTimer;
    private Transform player;
    private Rigidbody2D rb;
    public float moveSpeed = 2f;

    void Start()
    {
        shootTimer = shootingInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            Debug.LogError("Player not found!");
        }

        if (firePoint == null)
        {
            Debug.LogError("FirePoint not assigned!");
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= followRange)
            {
                // Calculate direction to the player
                Vector2 direction = (player.position - transform.position).normalized;
                // Set the enemy's velocity to move towards the player
                rb.velocity = direction * moveSpeed;
            }
            else
            {
                // Stop moving if the player is out of range
                rb.velocity = Vector2.zero;
            }

            shootTimer += Time.deltaTime;
            if (shootTimer >= shootingInterval)
            {
                if (distanceToPlayer <= shootingRange)
                {
                    Shoot();
                }
                shootTimer = 0f;
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null || player == null)
        {
            Debug.LogError("FirePoint or Player not assigned!");
            return;
        }

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.gravityScale = 0; // Ensure no gravity affects the bullet
        rbBullet.velocity = directionToPlayer * bulletSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
