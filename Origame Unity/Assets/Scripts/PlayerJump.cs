using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float jumpHeight;

    [SerializeField] private float normalGravity;
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
    [SerializeField] private Vector2 animGroundedCheckSize; //size for ground box check
    [SerializeField] private Transform animGroundedCheckPos; //position for ground box check

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerWalk walk;
    [SerializeField] private PlayerInteract interact;
    private Controls controls;

    [SerializeField] private AudioSource jumpAudio;

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
        anim.SetBool("grounded", Physics2D.OverlapBox(animGroundedCheckPos.position, animGroundedCheckSize, 0f, groundLayer));

        if (grounded)
        {
            if (!walk.SlopeAllowed)
            {
                rb.gravityScale = normalGravity;
            }
            else
            {
                if (Mathf.Abs(walk.SlopeAngle) < 0.5f)
                {
                    rb.gravityScale = normalGravity;
                }
                else
                {
                    rb.gravityScale = 0.1f;
                }
            }
        }
        else
        {
            interact.StopPull();

            if (rb.velocity.y < 0) //player is not on ground and vertical velocity is less than 0 - so falling
            {
                if (jumping) { jumping = false; } //if not on ground and start falling, not jumping anymore

                rb.gravityScale = fallGravity; //gravity is set higher than normal
            }
            else
            {
                rb.gravityScale = normalGravity;
            }
        }
    }

    private void Jump() //called by input events
    {
        if (grounded && !interact.IsPulling) //if player on ground
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight); //set vertical velocity to jump height
            jumping = true;
            grounded = false;

            jumpAudio.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize); //draw ground check box
    
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(animGroundedCheckPos.position, animGroundedCheckSize); //draw anim ground check box
    }
}
