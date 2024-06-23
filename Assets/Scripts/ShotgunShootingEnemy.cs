using UnityEngine;

public class ShotgunShootingEnemy : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float shootingInterval = 3f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 2f; // Speed of the bullet
    public int bulletCount = 3; // Number of bullets in a spread
    public float spreadAngle = 30f; // Total angle of the spread
    public LayerMask obstacleLayerMask; // Layer mask to include walls

    public float shootingRange = 4f; // Range within which the enemy can shoot the player
    private float shootTimer;
    public float followRange = 4f;

    private Transform player;
    private float nextShootTime;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextShootTime = Time.time + shootingInterval;
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

   /// void Update()
   // {
     //   if (player != null)
     //   {
            // Move towards the player slowly
    //        Vector2 direction = (player.position - transform.position).normalized;
     //       rb.velocity = direction * moveSpeed;
     //   }

     //   // Shoot at intervals
     //   if (Time.time >= nextShootTime)
        //{
     //       if (HasLineOfSight())
           // {
     //           Shoot();
     //       }
       //     nextShootTime = Time.time + shootingInterval;
     //   }
  //  }

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
        float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        float angleStep = spreadAngle / (bulletCount - 1);
        float startAngle = angleToPlayer - spreadAngle / 2;

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            rbBullet.gravityScale = 0; // Ensure no gravity affects the bullet
            Vector2 shootingDirection = bulletRotation * Vector2.right;
            rbBullet.velocity = shootingDirection * bulletSpeed;
        }
    }

    bool HasLineOfSight()
    {
        Vector2 directionToPlayer = player.position - firePoint.position;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, directionToPlayer, directionToPlayer.magnitude, obstacleLayerMask);
        return hit.collider == null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Prevent passing through walls and other enemies
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
