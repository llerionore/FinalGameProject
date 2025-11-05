using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float speed = 5f;
    float jumpForce = 4f;
    float input;
    bool isJumping = false;
    private Vector2 change;
    
    Rigidbody2D rb;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        input = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && !isJumping) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(speed * input, rb.velocity.y);

        if (change != Vector2.zero) {
            animator.SetFloat("moveL", change.x);
            animator.SetFloat("moveR", change.x);
        
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        isJumping = false;
    }
}
