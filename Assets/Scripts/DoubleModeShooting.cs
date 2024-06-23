using UnityEngine;
using System.Collections;

public class DoubleModeShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab for the bullet
    public GameObject flamePrefab; // Prefab for the flame projectile
    public GameObject shotgunBulletPrefab; // Prefab for the shotgun bullet
    public Transform firePoint;
    public float bulletSpeed = 2f; // Speed of the bullet
    public float shootingInterval = 2f;
    private float shootTimer;
    private Transform player;
    private Rigidbody2D rb;
    public float moveSpeed = 2f;

    public int maxHealth = 100;
    private int currentHealth;

    public float rapidFireInterval = 0.5f; // Interval for rapid fire mode
    private bool isRageMode = false;

    public float rageMultiplier = 1.6f; // Multiplier for rage mode

    public GameObject healthBarCanvas; // Reference to the health bar canvas
    private BossHP bossHP; // Reference to the BossHP script

    void Start()
    {
        shootTimer = shootingInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        bossHP = GetComponent<BossHP>();

        if (bossHP == null)
        {
            Debug.LogError("BossHP component not found!");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player not found!");
        }

        if (firePoint == null)
        {
            Debug.LogError("FirePoint not assigned!");
        }

        if (healthBarCanvas != null)
        {
            healthBarCanvas.SetActive(true); // Activate the health bar
        }
        else
        {
            Debug.LogError("Health bar canvas not assigned!");
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }

        shootTimer += Time.deltaTime;
        if (bossHP.currentHealth > bossHP.maxHealth * 0.5f)
        {
            if (shootTimer >= shootingInterval)
            {
                PerformAttack();
                shootTimer = 0f;
            }
        }
        else
        {
            if (!isRageMode)
            {
                EnterRageMode();
            }
            if (shootTimer >= shootingInterval / rageMultiplier)
            {
                PerformAttack();
                shootTimer = 0f;
            }
        }
    }

    void PerformAttack()
    {
        int attackType = Random.Range(0, 3);
        Debug.Log("Performing attack type: " + attackType);
        switch (attackType)
        {
            case 0:
                Shoot();
                break;
            case 1:
                StartCoroutine(FireFlameBurst());
                break;
            case 2:
                ShotgunAttack();
                break;
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

    IEnumerator FireFlameBurst()
    {
        if (firePoint == null || player == null)
        {
            Debug.LogError("FirePoint or Player not assigned!");
            yield break;
        }

        // Stop movement during flame burst attack
        rb.velocity = Vector2.zero;

        // Wait for 2 seconds
        Debug.Log("Preparing flame burst...");
        yield return new WaitForSeconds(2f); // Wait for 2 seconds

        // Fire 10 flame projectiles
        Debug.Log("Firing flame burst!");
        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        for (int i = 0; i < 10; i++)
        {
            GameObject flame = Instantiate(flamePrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rbFlame = flame.GetComponent<Rigidbody2D>();
            rbFlame.gravityScale = 0; // Ensure no gravity affects the flame
            rbFlame.velocity = directionToPlayer * bulletSpeed;
            yield return new WaitForSeconds(0.1f); // Slight delay between each flame
        }

        // Resume movement
        rb.velocity = (player.position - transform.position).normalized * moveSpeed;
    }

    void ShootFlame()
    {
        if (firePoint == null || player == null)
        {
            Debug.LogError("FirePoint or Player not assigned!");
            return;
        }

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        GameObject flame = Instantiate(flamePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbFlame = flame.GetComponent<Rigidbody2D>();
        rbFlame.gravityScale = 0; // Ensure no gravity affects the flame
        rbFlame.velocity = directionToPlayer * bulletSpeed;
    }

    void ShotgunAttack()
    {
        if (firePoint == null || player == null)
        {
            Debug.LogError("FirePoint or Player not assigned!");
            return;
        }

        // Spread angle for the shotgun bullets
        float spreadAngle = 45f;
        int bulletCount = 5;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = Mathf.Lerp(-spreadAngle / 2, spreadAngle / 2, i / (float)(bulletCount - 1));
            Vector3 direction = Quaternion.Euler(0, 0, angle) * (player.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(shotgunBulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            rbBullet.gravityScale = 0; // Ensure no gravity affects the bullet
            rbBullet.velocity = direction * bulletSpeed;
        }
    }

    void EnterRageMode()
    {
        isRageMode = true;
        Debug.Log("Boss entered rage mode!");
        // Reduce the shooting interval to make the boss shoot faster
        shootingInterval /= 2;
    }

    public void TakeDamage(int damage)
    {
        bossHP.TakeDamage(damage);
        if (bossHP.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss died!");
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
