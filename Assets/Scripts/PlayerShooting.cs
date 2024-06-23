using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public Animator animator;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    private float timeSinceLastShot = Mathf.Infinity;
    public float timeToReturnToIdle = 5f;

    private bool doubleDamage = false;  // Ensure this is declared at the class level

    public CooldownManager cooldownManager; // Reference to the CooldownManager

    void Update()
    {
        // Increment the timer continuously
        timeSinceLastShot += Time.deltaTime;

        // Check for shooting input
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
            timeSinceLastShot = 0f;  // Reset the timer on shooting
        }

        // Transition back to idle if enough time has passed since the last shot
        if (timeSinceLastShot >= timeToReturnToIdle)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("ReturnToIdle");
        }
    }

    void Shoot()
    {
        // Trigger the Attack animation
        animator.SetTrigger("Attack");
        animator.ResetTrigger("ReturnToIdle");

        // Calculate direction from the fire point to the mouse cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootingDirection = (mousePosition - firePoint.position).normalized; // Normalize the direction vector

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Rotate the bullet to face the direction of shooting
        float angle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg - 90; // Adjusting by -90 degrees if your bullet sprite is upwards by default
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Apply force to the bullet to move it towards the cursor
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(shootingDirection * bulletForce, ForceMode2D.Impulse);

        // Apply double damage if the flag is set
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null && doubleDamage)
        {
            bulletScript.damage *= 2;
        }
    }

    public void DoubleBulletDamage(float duration)
    {
        if (!cooldownManager.IsHammerOnCooldown())
        {
            StartCoroutine(DoubleBulletDamageCoroutine(duration));
            // Start the cooldown UI for the hammer ability
            cooldownManager.StartHammerDuration();
        }
    }

    private IEnumerator DoubleBulletDamageCoroutine(float duration)
    {
        doubleDamage = true;
        yield return new WaitForSeconds(duration);
        doubleDamage = false;
    }
}
