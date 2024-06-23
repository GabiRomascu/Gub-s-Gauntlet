using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float dashSpeed = 7f;
    public float dashDuration = 0.3f;
    private bool isDashing;
    private float dashTime;
    private Vector2 movement;
    public Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the Animator and SpriteRenderer components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found!");
        }
    }

    void Update()
    {
        // Capture movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Trigger dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            dashTime = dashDuration;
            animator.SetTrigger("Dash"); // Trigger the dash animation
        }

        // Update the animator with movement values
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Flip the sprite based on movement direction
        if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }

        // Trigger run animation if moving and not dashing
        if (movement.sqrMagnitude > 0 && !isDashing)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            if (dashTime > 0)
            {
                rb.velocity = movement.normalized * dashSpeed;
                dashTime -= Time.fixedDeltaTime;
            }
            else
            {
                isDashing = false;
                rb.velocity = Vector2.zero; // Stop dashing movement after duration ends
            }
        }
        else
        {
            // Apply normal movement
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public IEnumerator ApplySpeedBoost(float multiplier, float duration)
    {
        moveSpeed *= multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = 5f;
    }
}
