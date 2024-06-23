using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Add this using directive

public class Boss : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currentHealth;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 2f; // Speed of the bullet
    public float shootingInterval = 2f;
    private float shootTimer;
    private Transform player;
    private Rigidbody2D rb;
    public float moveSpeed = 2f;

    public float rapidFireInterval = 0.5f; // Interval for rapid fire mode
    private bool isRapidFireMode = false;

    // Add the healthBarCanvas and healthText fields
    public GameObject healthBarCanvas;
    public Text healthText;

    private BossHealthBarController healthBarController;
    private Animator animator; // Reference to the Animator component

    // Add the Boss health bar UI element
    public Image bossHealthBar;

    void Start()
    {
        shootTimer = shootingInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); // Get the Animator component

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
            healthBarController = healthBarCanvas.GetComponent<BossHealthBarController>();
            healthBarController.SetMaxHealth(maxHealth);
            healthBarCanvas.SetActive(false); // Hide health bar at start
        }

        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }

        UpdateHealthBar(); // Update the health bar at start
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }

        shootTimer += Time.deltaTime;
        if (currentHealth > maxHealth * 0.5f)
        {
            if (shootTimer >= shootingInterval)
            {
                Attack();
                shootTimer = 0f;
            }
        }
        else
        {
            isRapidFireMode = true;
            if (shootTimer >= rapidFireInterval)
            {
                RapidFire();
                shootTimer = 0f;
            }
        }
    }

    void Attack()
    {
        if (firePoint == null || player == null)
        {
            Debug.LogError("FirePoint or Player not assigned!");
            return;
        }
        animator.SetTrigger("Attack"); // Trigger the attack animation

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.gravityScale = 0; // Ensure no gravity affects the bullet
        rbBullet.velocity = directionToPlayer * bulletSpeed;
    }

    void RapidFire()
    {
        if (firePoint == null || player == null)
        {
            Debug.LogError("FirePoint or Player not assigned!");
            return;
        }

        animator.SetTrigger("Attack"); // Trigger the attack animation

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        for (int i = 0; i < 3; i++) // Fire three bullets in quick succession
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            rbBullet.gravityScale = 0; // Ensure no gravity affects the bullet
            rbBullet.velocity = directionToPlayer * (bulletSpeed + i); // Increase speed slightly for each bullet
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        if (healthBarController != null)
        {
            healthBarController.SetHealth(currentHealth);
        }

        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }

    void UpdateHealthBar()
    {
        // Ensure bossHealthBar is not null to avoid errors
        if (bossHealthBar != null)
        {
            bossHealthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Boss died!");
        animator.SetTrigger("Die"); // Trigger the death animation
        StartCoroutine(DestroyAfterDelay(10f)); // Start coroutine to destroy after 10 seconds
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
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
