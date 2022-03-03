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
    [SerializeField] private float acceleration; 
    [SerializeField] private float deceleration; 
    private bool facingRight = true;
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
        if (!jump.Grounded)
        {
            footstepsAudio.Pause();
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            if (!footstepsAudio.isPlaying)
            {
                footstepsAudio.Play();
            }
        }
        else
        {
            footstepsAudio.Pause();
        }



        slopeAllowed = SlopeCheck();

        if (Mathf.Abs(moveInput - moveInputSmoothed) < 0.2f)
        {
            moveInputSmoothed = moveInput;
        }
        else
        {
            if (moveInput == 0)
            {
                Debug.Log("Decelerate");
                moveInputSmoothed = Mathf.Clamp((float)Math.Round(Mathf.Lerp(moveInputSmoothed, moveInput, deceleration * Time.deltaTime), 2), -1, 1);
            }
            else
            {
                Debug.Log("Accelerate");
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
            rb.velocity = Quaternion.Euler(0, 0, slopeAngle) * Vector2.right *  moveInputSmoothed * maxSpeed;
        }
        else 
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
