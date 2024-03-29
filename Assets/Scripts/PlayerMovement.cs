using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Basic Movement")]
    public float horizontal; // Holds values of the virtual axis between -1 to 1
    public float speed = 8.0f;
    public float jumpForce = 16.0f;
    public bool isFacingRight = true;

    [Header("Coyote Jump")]
    public bool isJumping;
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    public float jumpBufferCounter;

    [Header("Dashing")]
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing;
    public float dashingPower = 24.0f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1.0f;

    [Header("Wall Jump")]
    [SerializeField] private bool isWallJumping;
    [SerializeField] private bool isWallSliding;
    private float wallSlideSpeed = 2.0f;
    private float wallJumpDirection;
    private float wallJumpCounter;
    public float wallJumpTime = 2.0f;
    public float wallJumpDuration = 0.4f;
    public Vector2 wallJumpPower = new Vector2(8.0f, 16.0f);

    [Header("Shooting")]
    public int maxAmmo = 3;
    public int currentAmmo;
    public Projectile Projectile;

    [Header("Everything Else")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer; //Allows use of layers in Unity Editor
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform firePoint;

    public Animator animator;


    private bool isGrounded() //Creates an invisible circle of r = 0.2 that returns true if it intersects with a groundLayer
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (isWalled() && !isGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void Flip() //Just flips the player gameObject so that animations are easier to create
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localscale = transform.localScale;
            localscale.x *= -1f; //Multiplies the x component of the localscale by -1 to flip
            transform.localScale = localscale;
        }
    }

    private void Start()
    {
        currentAmmo = 0;
    }

    void Update() //Executes every frame
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            jumpBufferCounter = 0f;

            StartCoroutine(JumpCooldown());
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }

        if (rb.position.y <= -18.0f)
        {
            transform.Translate(0f, 0f, 0f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

    }

    void FixedUpdate() //Executed at a specific rate which can be changed in the Editor
    {
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpCounter = wallJumpTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpCounter = 0f;

            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpDirection);
        }
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        animator.SetBool("Jumping", true);
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
        animator.SetBool("Jumping", false);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private bool CanShoot()
    {
        return isGrounded() && !isWalled() && !isDashing;
    }    

    private void Shoot()
    {
        if (currentAmmo > 0 && CanShoot())
        {
            Projectile projectileInstance = Instantiate(Projectile, firePoint.position, Quaternion.identity);
            Vector3 shootDirection = isFacingRight ? Vector3.right : Vector3.left;
            projectileInstance.SetDirection(shootDirection);
            currentAmmo--;
        }
    }

    public void AddAmmo()
    {
        if (currentAmmo < maxAmmo)
        {
            currentAmmo++;
        }
    }
}