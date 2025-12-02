using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Basic Movement and Jumping")]
    public float speed = 5f;
    public float jumpForce = 4f;
    bool isJumping;

    [Header("Dash Settings")]
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


    float moveInput;
    Rigidbody2D rb;
    Animator animator;
    TrailRenderer trail;

    void Start()
    {
        BashTimeReset = BashTime;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        trail = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Dash") && canDash)
        {
            StartDash();
        }

        if (isDashing) return;

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
        Bash();
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
    }

    void StartDash()
    {
        canDash = false;
        isDashing = true;
        trail.emitting = true;

  
        dashDirection = new Vector2(moveInput, 0);


        if (dashDirection.x == 0)
            dashDirection.x = transform.localScale.x;

        StartCoroutine(StopDash());
    }

    IEnumerator StopDash()
    {
        yield return new WaitForSeconds(dashTime);

        isDashing = false;
        trail.emitting = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        isJumping = false;
        canDash = true;
    }

    void Bash()
    {
        RaycastHit2D[] Rays = Physics2D.CircleCastAll(transform.position, Raduis, Vector3.forward);
        foreach (RaycastHit2D ray in Rays)
        {

            NearToBashAbleObj = false;

            if (ray.collider.tag == "Bouncable")
            {
                NearToBashAbleObj = true;
                BashAbleObj = ray.collider.transform.gameObject;
                break;
            }
        }
        if (NearToBashAbleObj)
        {
            BashAbleObj.GetComponent<SpriteRenderer>().color = Color.red;
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Time.timeScale = 0;
                BashAbleObj.transform.localScale = new Vector2(1.4f, 1.4f);
                Arrow.SetActive(true);
                Arrow.transform.position = BashAbleObj.transform.transform.position;
                IsChosingDir = true;
            }
            else if (IsChosingDir && Input.GetKeyUp(KeyCode.Mouse1))
            {
                Time.timeScale = 1f;
                BashAbleObj.transform.localScale = new Vector2(1, 1);
                IsChosingDir = false;
                IsBashing = true;
                rb.velocity = Vector2.zero;
                transform.position = BashAbleObj.transform.position + new Vector3(0, 0.2f, 0); ;
                BashDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                BashDir.z = 0;
                
                BashDir = BashDir.normalized;
                BashAbleObj.GetComponent<Rigidbody2D>().AddForce(-BashDir * 50, ForceMode2D.Impulse);
                Arrow.SetActive(false);

            }
        }
        else if (BashAbleObj != null)
        {
            BashAbleObj.GetComponent<SpriteRenderer>().color = Color.white;
        }

        ////// Preform the bash
        ///
        if (IsBashing)
        {
            if (BashTime > 0)
            {
                BashTime -= Time.deltaTime;
                rb.velocity = BashDir * BashPower * Time.deltaTime;
            }
            else
            {
                IsBashing = false;
                BashTime = BashTimeReset;
                rb.velocity = new Vector2(rb.velocity.x, 0);


            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Raduis);
    }
}
