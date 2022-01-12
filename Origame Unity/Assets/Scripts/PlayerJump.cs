using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float normalGravity;
    public float NormalGravity
    {
        get { return normalGravity; }
    }

    [SerializeField] private float fallGravity;

    //ground check
    private bool grounded;
    public bool Grounded
    {
        get { return grounded; }
    }

    private bool jumping;
    public bool Jumping
    {
        get { return jumping; }
    }
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer; //ground layer

    [SerializeField] private Vector2 groundCheckSize; //size for ground box check
    [SerializeField] private Transform groundCheckPos; //position for ground box check

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerWalk walk;
    private Controls controls;

    private void Awake()
    {
        controls = new Controls(); //initialise controls

        controls.Player.Jump.performed += _ => Jump(); //when jump button is pressed, do Jump()
    }

    private void OnEnable()
    {
        controls.Player.Jump.Enable(); //enable jump controls
    }

    private void OnDisable()
    {
        controls.Player.Jump.Disable(); //disable jump controls
    }

    private void FixedUpdate()
    {
        //check if the player is grounded by casting a box with size groundCheckSize and position groundCheckPos for the layer groundLayer
        grounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);

        if (grounded)
        {
            if (walk.SlopeAllowed)
            {
                rb.gravityScale = 0f;
            }
            else
            {
                rb.gravityScale = normalGravity;
            }
        }
        else if (rb.velocity.y < 0) //player is not on ground and vertical velocity is less than 0 - so fallign
        {
            if (jumping) { jumping = false; } //if not on ground and start falling, not jumping anymore

            rb.gravityScale = fallGravity; //gravity is set higher than normal
        }
    }

    private void Jump() //called by input events
    {
        if (grounded) //if player on ground
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight); //set vertical velocity to jump height
            jumping = true;
            grounded = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize); //draw ground check box
    }
}
