using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float jumpForce = 8.5f;

    [SerializeField] private float normalGravity = 3;
    [SerializeField] private float fallGravity = 5;

    //ground check
    private bool isGrounded;
    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    private bool isJumping;
    public bool IsJumping
    {
        get { return isJumping; }
    }

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer; //ground layer

    [SerializeField] private Vector2 groundCheckSize; //size for ground box check
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.5f); //position for ground box check

    private const float groundCheckWaitTime = 0.1f;
    private bool canCheckGround = true;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerWalk walk;
    [SerializeField] private PlayerInteract interact;
    private Controls controls;

    [SerializeField] private AudioSource jumpAudio;

    private void Awake() {
        controls = new Controls(); //initialise controls

        controls.Player.Jump.performed += _ => Jump(); //when jump button is pressed, do Jump()
    }

    private void OnEnable() {
        controls.Player.Jump.Enable(); //enable jump controls
    }

    private void OnDisable() {
        controls.Player.Jump.Disable(); //disable jump controls
    }


    private void FixedUpdate() {
        if (canCheckGround) {
            //check if the player is grounded by casting a box with size groundCheckSize and position groundCheckPos for the layer groundLayer
            isGrounded = Physics2D.OverlapBox((Vector2) transform.position + groundCheckOffset, groundCheckSize, 0f, groundLayer);
            anim.SetBool("grounded", isGrounded);
        }

        if (isGrounded) {
            isJumping = false;

            if (walk.OnSlope() && !walk.IsMoving()) {
                rb.gravityScale = 0;
            } else {
                rb.gravityScale = normalGravity;
            }
        } else {
            interact.StopPull();

            if (rb.velocity.y < 0) { //player is not on ground and vertical velocity is less than 0; falling
                //not on ground and start falling; not jumping anymore
                isJumping = false;
                rb.gravityScale = fallGravity; //gravity is set higher than normal
            } else {
                rb.gravityScale = normalGravity;
            }
        }
    }

    //called by input events
    private void Jump() { 
        if (isGrounded && walk.SlopeAllowed() && !interact.IsPulling) { //if player on valid ground
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); //set vertical velocity to jump height
         
            isJumping = true;
            isGrounded = false;

            canCheckGround = false;
            Invoke(nameof(EnableGroundCheck), groundCheckWaitTime);

            jumpAudio.Play();
        }
    }

    private void EnableGroundCheck() {
        canCheckGround = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + (Vector3) groundCheckOffset, groundCheckSize); //draw ground check box
    }
}
