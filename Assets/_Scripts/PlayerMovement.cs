using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Basic Movement")]
    public float speed = 5f;
    public float jumpForce = 4f;
    bool isJumping;
    public float fallGravityMultiplier = 2f;
    public float lowJumpGravityMultiplier = 2f;
    float moveInput;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashTime = 0.15f;
    bool isDashing;
    bool canDash = true;
    Vector2 dashDirection;

    [Header("Bash")]
    [SerializeField] private float Raduis;
    [SerializeField] GameObject BashAbleObj;
    private bool NearToBashAbleObj;
    private bool IsChosingDir;
    private bool IsBashing;
    [SerializeField] private float BashPower;
    [SerializeField] private float BashTime;
    [SerializeField] private GameObject Arrow;
    Vector3 BashDir;
    private float BashTimeReset;

    [Header("Invincibility")]
    private int originalLayer;
    private bool ghostEnabled = false;
    public bool isInvincible = false;

    Rigidbody2D rb;
    Animator animator;
    TrailRenderer trail;
    MapLayerManager mapManager;

    void Start()
    {
        BashTimeReset = BashTime;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        trail = GetComponent<TrailRenderer>();
        mapManager = FindObjectOfType<MapLayerManager>();

        originalLayer = gameObject.layer;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Dash") && canDash)
        {
            StartDash();
        }

        if (isDashing) return;

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping || Input.GetKeyDown(KeyCode.W) && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }

        Bash();

        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        animator.SetFloat("moveInput", moveInput);

        bool isWalking = Mathf.Abs(moveInput) > 0.1f && !isJumping && !isDashing && !IsBashing;
        animator.SetBool("isWalking", isWalking);

        bool isFalling = isJumping && rb.velocity.y < -0.1f;
        animator.SetBool("isFalling", isFalling);

        bool isRising = isJumping && rb.velocity.y > 0.1f;
        animator.SetBool("isJumping", isRising);

        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isBashing", IsBashing);

        if (moveInput > 0)
            transform.localScale = new Vector3(2, 2, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-2, 2, 1);
    }

    void FixedUpdate()
    {
        if (IsBashing)
            return;

        if (isDashing)
        {
            rb.velocity = dashDirection * dashSpeed;
            return;
        }

        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // weak jump
            rb.gravityScale = lowJumpGravityMultiplier;
        }
        else if (rb.velocity.y < 0)
        {
            // falling
            rb.gravityScale = fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    void StartDash()
    {
        canDash = false;
        isDashing = true;
        trail.emitting = true;

        dashDirection = new Vector2(moveInput, 0);

        if (dashDirection.x == 0)
            dashDirection.x = transform.localScale.x;

        animator.SetBool("isDashing", true);

        StartCoroutine(StopDash());
    }

    IEnumerator StopDash()
    {
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        animator.SetBool("isDashing", false);
        trail.emitting = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.normal.y > 0.6f)
            {
                isJumping = false;
                canDash = true;
                return;
            }
        }
    }

    void Bash()
    {
        RaycastHit2D[] Rays = Physics2D.CircleCastAll(transform.position, Raduis, Vector3.forward);
        foreach (RaycastHit2D ray in Rays)
        {
            NearToBashAbleObj = false;
            BashAbleObj = null;

            if (ray.collider.CompareTag("Bouncable"))
            {
                NearToBashAbleObj = true;
                BashAbleObj = ray.collider.transform.gameObject;
                break;
            }
        }

        if (!NearToBashAbleObj && Arrow.activeSelf)
            Arrow.SetActive(false);

        if (NearToBashAbleObj)
        {
            ColorUtility.TryParseHtmlString("#40e0d0", out Color c);
            BashAbleObj.GetComponent<SpriteRenderer>().color = c;

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Time.timeScale = 0;
                BashAbleObj.transform.localScale = new Vector2(1f, 1f);
                Arrow.SetActive(true);
                Arrow.transform.position = BashAbleObj.transform.position;
                IsChosingDir = true;
            }
            else if (IsChosingDir && Input.GetKeyUp(KeyCode.Mouse1))
            {
                Time.timeScale = 1f;
                BashAbleObj.transform.localScale = new Vector2(1, 1);
                IsChosingDir = false;
                IsBashing = true;

                isInvincible = true;

                EnableGhostMode();

                animator.SetBool("isBashing", true);

                rb.velocity = Vector2.zero;
                transform.position = BashAbleObj.transform.position + new Vector3(0, 0.2f, 0);

                BashDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                BashDir.z = 0;
                BashDir = BashDir.normalized;

                BashDir = (BashDir + Vector3.up * 0.15f).normalized;

                rb.AddForce(BashDir * BashPower, ForceMode2D.Impulse);

                var objRb = BashAbleObj.GetComponent<Rigidbody2D>();
                if (objRb != null)
                    objRb.AddForce(-BashDir * (BashPower * 0.7f), ForceMode2D.Impulse);

                if (mapManager != null)
                {
                    mapManager.SwitchToNextLayer();
                }
            }
        }
        else if (BashAbleObj != null)
        {
            BashAbleObj.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (IsBashing)
        {
            BashTime -= Time.deltaTime;
            rb.velocity = BashDir * BashPower;

            if (BashTime <= 0)
            {
                DisableGhostMode();

                IsBashing = false;
                isInvincible = false;
                BashTime = BashTimeReset;

                animator.SetBool("isBashing", false);

                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.3f);
            }
        }
    }

    void EnableGhostMode()
    {
        if (ghostEnabled) return;

        ghostEnabled = true;
        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
    }

    void DisableGhostMode()
    {
        if (!ghostEnabled) return;

        ghostEnabled = false;
        gameObject.layer = originalLayer;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Raduis);
    }
}
