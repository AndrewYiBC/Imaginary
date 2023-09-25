using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // Variables
    // Components
    private Rigidbody2D rb;

    // Movement
    [SerializeField] private float moveSpeed;
    private float moveInputHorizontal = 0;
    private bool isFacingRight = true;

    // Jump
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    // Scripts
    private SkillPolar polar;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        polar = GetComponent<SkillPolar>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Movement
        rb.velocity = new Vector2(moveInputHorizontal * moveSpeed, rb.velocity.y);
        if ((isFacingRight && moveInputHorizontal < 0) || (!isFacingRight && moveInputHorizontal > 0))
        {
            Flip();
        }
    }



    // Input System
    // Movement
    public void Move(InputAction.CallbackContext context)
    {
        moveInputHorizontal = context.ReadValue<Vector2>().x;
    }

    // Jump
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !polar.IsInPolar())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }



    // Helper Functions
    // Movement
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
    }

    // Jump
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
    }

}
