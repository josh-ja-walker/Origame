using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalk : MonoBehaviour
{
    //walking
    [Header("Walk")] //header in inspector
    [SerializeField] private float maxSpeed; //magnitude of velocity in the horizontal direction when moving at max speed
    private float moveInput; //the value of horizontal inputs (-1 for A or left arrow, 1 for D or right arrow)
    private float moveInputSmoothed; //value of moveInput but accounting for acceleration and deceleration
    [SerializeField] private float acceleration = 10; 
    [SerializeField] private float deceleration = 17.5f; 
    
    [SerializeField] private Transform[] shouldFlip;

    private bool facingRight = true;
    public bool FacingRight { get { return facingRight; } }


    //slopes
    [Header("Slopes")]
    [SerializeField] private float maxSlopeAngle = 65f; //max slope angle the player can walk up
    private const float SLOPE_THRESHOLD = 0.5f; //threshold of what constitutes a slope
    private float slopeAngle; //angle of current slope

    [SerializeField] private Vector2 leftRayOffset = new Vector2(-0.3125f, -0.225f); //position for left slope ray
    [SerializeField] private float leftRayDist = 1f; //distance for rays
    [SerializeField] private Vector2 rightRayOffset = new Vector2(0.3125f, -0.225f); //position for right slope ray
    [SerializeField] private float rightRayDist = 0.5f; //distance for rays
    [SerializeField] private LayerMask groundLayer; //ground layer
    private SlopeState slopeState;
    
    enum SlopeState {
        SlopeNotFound,
        SlopeAllowed,
        SlopeNotAllowed,
    }


    //references
    [Header("References")]
    [SerializeField] private Rigidbody2D rb; //reference to the Rigidbody2D component
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerInteract interact;

    private Controls controls; //reference to the controls

    [SerializeField] private AudioSource footstepsAudio;

    private void Awake() { //run before start
        controls = new Controls(); //initialise controls
        controls.Player.Walk.performed += ctx => StartWalk(ctx); //when walk is performed, pass ctx to walk method
        controls.Player.Walk.canceled += _ => CancelWalk(); //when walk is canceled, call walk again to set moveInput to 0
    }

    private void OnEnable() { //run when component this script is attached to is enabled
        controls.Player.Walk.Enable(); //enable walk when this script is enabled
    }

    private void OnDisable() { //run when component this script is attached to is disabled
        controls.Player.Walk.Disable(); //disable walk when this script is disabled
    }

    private void Update() {
        /* If player is moving play footsteps sfx */
        if (Mathf.Abs(rb.velocity.x) > 0.1 && !footstepsAudio.isPlaying) {
            footstepsAudio.Play();
        } else {
            footstepsAudio.Pause();
        }

        slopeState = SlopeCheck(); //check if slope can be walked on

        if (Mathf.Abs(moveInput - moveInputSmoothed) < 0.2f) { //if difference between moveInput and the smoothed moveInput is less than 0.2
            moveInputSmoothed = moveInput; //remove smoothing - moveInputSmoothed and moveInput are equal
        } else { //not at max speed - i.e., still smoothing as moveInputSmoothed != moveInput
            //smoothed input is clamped between -1 and 1
            //smoothed input is the value given by the lerp function, which linearly interpolated between current value and target value (0) at speed of deceleration value
            //smoothed input is rounded to 2 dp
            float lerpedMove = Mathf.Lerp(moveInputSmoothed, moveInput, (moveInput != 0 ? acceleration : deceleration) * Time.deltaTime);
            moveInputSmoothed = Mathf.Clamp((float) Math.Round(lerpedMove, 2), -1, 1);
        }

        if (!interact.IsPulling) {
            if (moveInputSmoothed < 0 && facingRight) {
                Flip();
            } else if (moveInputSmoothed > 0 && !facingRight) {
                Flip();
            }
        }
    }

    private void FixedUpdate() { //run every fixed frame update to ensure physics is consistent (framerate does not affect physics calcs)
        if (OnSlope() && jump.IsGrounded) {
            if (SlopeAllowed() && !jump.IsJumping) {
                rb.velocity = Quaternion.Euler(0, 0, slopeAngle) * Vector2.right * moveInputSmoothed * maxSpeed;
            } else {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y > 0 ? 0 : rb.velocity.y);
            }
        } else {
            rb.velocity = new Vector2(moveInputSmoothed * maxSpeed, rb.velocity.y);
        }
    }

    private void StartWalk(InputAction.CallbackContext _ctx) {
        moveInput = _ctx.ReadValue<float>(); //read axis value from the ctx
        anim.SetFloat("moveX", Mathf.Abs(moveInput));
    }

    private void CancelWalk() {
        anim.SetFloat("moveX", 0);
        moveInput = 0;

        footstepsAudio.Pause();
    }

    private void Flip() {
        facingRight = !facingRight;

        foreach (Transform objectToFlip in shouldFlip) {
            objectToFlip.Rotate(0, 180f, 0);
        }
    }

    private SlopeState SlopeCheck() {
        //shoot raycast from left side
        RaycastHit2D hitLeft = Physics2D.Raycast((Vector2) transform.position + leftRayOffset, Vector2.down, leftRayDist, groundLayer); 
        //shoot raycast from right side
        RaycastHit2D hitRight = Physics2D.Raycast((Vector2) transform.position + rightRayOffset, Vector2.down, rightRayDist, groundLayer); 

        //calc angles - angle from normal to vertical is same as slope to horizontal
        //if angle is > 0, then slope is on the right, if angle < 0 then slope is on left
        float angleLeft = -Vector2.SignedAngle(hitLeft.normal, Vector2.up);
        float angleRight = -Vector2.SignedAngle(hitRight.normal, Vector2.up); 

        if (hitLeft && hitRight) { //if both rays hit
            slopeAngle = angleLeft > angleRight ? angleRight : angleLeft; // take minimum angle
            // slopeAngle = (angleLeft + angleRight) / 2f; //average out slope angles
        } else if (hitLeft || hitRight) { //if one of hit 
            slopeAngle = hitLeft ? angleLeft : angleRight; //returns the angle greater than 0
        } else {
            /* Neither hit */
            return SlopeState.SlopeNotFound;
        }

        return Mathf.Abs(slopeAngle) < maxSlopeAngle ? SlopeState.SlopeAllowed : SlopeState.SlopeNotAllowed;
    }

    public bool IsMoving() { return moveInputSmoothed != 0; }
    public bool SlopeAhead() { return Mathf.Sign(slopeAngle) == Mathf.Sign(moveInput); }
    public bool SlopeAllowed() { return slopeState == SlopeState.SlopeAllowed; }
    public bool OnSlope() { return SlopeFound() && Mathf.Abs(slopeAngle) >= SLOPE_THRESHOLD; }
    public bool SlopeFound() { return slopeState != SlopeState.SlopeNotFound; }

    private void OnDrawGizmosSelected() { //runs when game object selected in editor
        Gizmos.color = Color.blue;
        
        //draw left slope ray
        Vector3 leftPos = transform.position + (Vector3) leftRayOffset;
        Gizmos.DrawSphere(leftPos, 0.025f);
        Gizmos.DrawLine(leftPos, leftPos + Vector3.down * leftRayDist); 

        //draw right slope ray
        Vector3 rightPos = transform.position + (Vector3) rightRayOffset;
        Gizmos.DrawSphere(rightPos, 0.025f);
        Gizmos.DrawLine(rightPos, rightPos + Vector3.down * rightRayDist); 
    }
}
