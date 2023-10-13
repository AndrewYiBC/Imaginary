using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // Variables
    // GameObjects
    private GameObject playerObject;

    // Components
    private Rigidbody2D rb;

    // Movement
    [SerializeField] private float moveSpeed;
    private float moveInputHorizontal = 0;
    private bool isFacingRight = true;

    // Jump
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform[] groundCheckTransforms;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    // Gravity
    private bool isInPositiveHalf = true;
    private int groundCheckTransformIndex = 0;

    // Scripts
    private SkillPolar polar;



    void Start()
    {
        playerObject = transform.parent.gameObject;
        rb = playerObject.GetComponent<Rigidbody2D>();
        polar = GetComponent<SkillPolar>();
    }

    void Update()
    {
        GravityCheck();
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
        moveInputHorizontal = context.ReadValue<float>();
    }

    // Jump
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !polar.IsInPolar())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        bool isRising = (isInPositiveHalf && rb.velocity.y > 0f) || (!isInPositiveHalf && rb.velocity.y < 0f);
        if (context.canceled && isRising)
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
        return Physics2D.OverlapCircle(groundCheckTransforms[groundCheckTransformIndex].position, groundCheckRadius, groundLayer);
    }

    // Gravity
    private void GravityCheck()
    {
        if ((isInPositiveHalf && transform.position.y < 0) || (!isInPositiveHalf && transform.position.y > 0))
        {
            FlipGravity();
        }
    }

    private void FlipGravity()
    {
        isInPositiveHalf = !isInPositiveHalf;
        rb.gravityScale = -rb.gravityScale;
        jumpForce = -jumpForce;
        groundCheckTransformIndex = 1 - groundCheckTransformIndex;
    }

}
