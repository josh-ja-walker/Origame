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
    public float MoveInput
    {
        get { return moveInput; }
    }

    private float moveInputSmoothed; //value of moveInput but accounting for acceleration and deceleration
    [SerializeField] private float acceleration; 
    [SerializeField] private float deceleration; 
    private bool facingRight = true;
    public bool FacingRight
    {
        get { return facingRight; }
    }
    [SerializeField] private Transform[] shouldFlip;

    //slopes
    [Header("Slopes")]
    [SerializeField] private float maxSlopeAngle; //max slope angle the player can walk up
    private float slopeAngle; //angle of current slope
    public float SlopeAngle
    {
        get { return slopeAngle; }
    }
    [SerializeField] private Transform leftRayPos; //position for left slope ray
    [SerializeField] private Transform rightRayPos; //position for right slope ray
    [SerializeField] private float rayDist; //distance for rays
    [SerializeField] private LayerMask groundLayer; //ground layer
    private bool slopeAllowed;
    public bool SlopeAllowed
    {
        get { return slopeAllowed; }
    }

    //references
    [Header("References")]
    [SerializeField] private Rigidbody2D rb; //reference to the Rigidbody2D component
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerInteract interact;

    private Controls controls; //reference to the controls

    [SerializeField] private AudioSource footstepsAudio;

    private void Awake() //run before start
    {
        controls = new Controls(); //initialise controls
        controls.Player.Walk.performed += ctx => StartWalk(ctx); //when walk is performed, pass ctx to walk method
        controls.Player.Walk.canceled += _ => CancelWalk(); //when walk is canceled, call walk again to set moveInput to 0
    }

    private void OnEnable() //run when component this script is attached to is enabled
    {
        controls.Player.Walk.Enable(); //enable walk when this script is enabled
    }

    private void OnDisable() //run when component this script is attached to is disabled
    {
        controls.Player.Walk.Disable(); //disable walk when this script is disabled
    }

    private void Update()
    {
        if (!jump.Grounded || Time.timeScale != 1) //if player is in air or game is paused
        {
            footstepsAudio.Pause(); //pause the footsteps
        } 
        else if (Mathf.Abs(rb.velocity.x) > 0.1f) //else if the player is moving 
        {
            if (!footstepsAudio.isPlaying) //if not already playing
            {
                footstepsAudio.Play(); //play the footsteps
            }
        }
        else //else if player is not moving
        {
            footstepsAudio.Pause(); //pause the footsteps
        }

        slopeAllowed = SlopeCheck(); //check if slope can be walked on

        if (Mathf.Abs(moveInput - moveInputSmoothed) < 0.2f) //if difference between moveInput and the smoothed moveInput is less than 0.2
        {
            moveInputSmoothed = moveInput; //remove smoothing - moveInputSmoothed and moveInput are equal
        }
        else //not at max speed - i.e., still smoothing as moveInputSmoothed != moveInput
        {
            if (moveInput == 0) //if decelerating - target moveInput is 0
            {
                //smoothed input is clamped between -1 and 1
                //smoothed input is the value given by the lerp function, which linearly interpolated between current value and target value (0) at speed of deceleration value
                //smoothed input is rounded to 2 dp
                moveInputSmoothed = Mathf.Clamp((float)Math.Round(Mathf.Lerp(moveInputSmoothed, moveInput, deceleration * Time.deltaTime), 2), -1, 1);
            }
            else //accelerating - target moveInput is not 0 (player has given input)
            {

                //smooth the input but with speed of acceleration instead
                moveInputSmoothed = Mathf.Clamp((float)Math.Round(Mathf.Lerp(moveInputSmoothed, moveInput, acceleration * Time.deltaTime), 2), -1, 1);
            }
        }

        if (!interact.IsPulling)
        {
            if (moveInputSmoothed < 0 && facingRight)
            {
                Flip();
            }
            else if (moveInputSmoothed > 0 && !facingRight)
            {
                Flip();
            }
        }
    }

    private void FixedUpdate() //run every fixed frame update to ensure physics is consistent (framerate does not affect physics calcs)
    {
        if (jump.Grounded && !jump.Jumping && Mathf.Abs(slopeAngle) > 0f && slopeAllowed)
        {
            rb.velocity = Quaternion.Euler(0, 0, slopeAngle) * Vector2.right * moveInputSmoothed * maxSpeed;
        }
        else if (slopeAllowed || !jump.Grounded)
        {
            rb.velocity = new Vector2(moveInputSmoothed * maxSpeed, rb.velocity.y);
        }
    }

    private void StartWalk(InputAction.CallbackContext _ctx)
    {
        moveInput = _ctx.ReadValue<float>(); //read axis value from the ctx
        anim.SetFloat("moveX", Mathf.Abs(moveInput));
    }

    private void CancelWalk()
    {
        anim.SetFloat("moveX", 0);
        moveInput = 0;

        if (jump.Grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        footstepsAudio.Pause();
    }

    private void Flip()
    {
        facingRight = !facingRight;

        foreach (Transform objectToFlip in shouldFlip)
        {
            objectToFlip.Rotate(0, 180f, 0);
        }
    }

    private bool SlopeCheck()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(leftRayPos.position, Vector2.down, rayDist, groundLayer); //shoot raycast from left side
        RaycastHit2D hitRight = Physics2D.Raycast(rightRayPos.position, Vector2.down, rayDist, groundLayer); //shoot raycast from right side

        //calc angles - angle from normal to vertical is same as slope to horizontal
        //if angle is > 0, then slope is on the right, if angle < 0 then slope is on left
        float angleLeft = -Vector2.SignedAngle(hitLeft.normal, Vector2.up);
        float angleRight = -Vector2.SignedAngle(hitRight.normal, Vector2.up); 

        if (hitLeft && hitRight) //if both rays hit
        {
            slopeAngle = (angleLeft + angleRight) / 2f; //average out slope angles
        }
        else if (hitLeft || hitRight) //if one of hit
        {
            slopeAngle = hitLeft ? angleLeft : angleRight; //returns the angle greater than 0
        }
        else
        {
            return false;
        }

        if (Mathf.Abs(slopeAngle) > maxSlopeAngle) //if slope is greater than max slope angle
        {
            return Mathf.Sign(slopeAngle) != Mathf.Sign(moveInput); 
        }

        return true; //slope is shallower than max angle
    }

    private void OnDrawGizmosSelected() //runs when game object selected in editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftRayPos.position, leftRayPos.position + Vector3.down * rayDist); //draw left slope ray
        Gizmos.DrawLine(rightRayPos.position, rightRayPos.position + Vector3.down * rayDist); //draw right slope ray
    }
}
