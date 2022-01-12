using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalk : MonoBehaviour
{

    //walking
    [Header("Walk")] //header in inspector
    [SerializeField] private float maxSpeed; //magnitude of velocity in the horizontal direction when moving at max speed
    private float currSpeedMult; 
    [SerializeField] private float acceleration; 
    [SerializeField] private float deceleration; 
    [SerializeField] private bool decelerating; 
    private float moveInput; //the value of horizontal inputs (-1 for A or left arrow, 1 for D or right arrow)
    private bool stopped; //ensures one time

    //slopes
    [Header("Slopes")]
    [SerializeField] private float maxSlopeAngle; //max slope angle the player can walk up
    private float slopeAngle; //angle of current slope
    [SerializeField] private Transform leftRayPos; //position for left slope ray
    [SerializeField] private Transform rightRayPos; //position for right slope ray
    [SerializeField] private float rayDist; //distance for rays
    [SerializeField] private LayerMask groundLayer; //ground layer
    [SerializeField] private PhysicsMaterial2D groundMaterial;
    private bool slopeAllowed;
    public bool SlopeAllowed
    {
        get { return slopeAllowed; }
    }
    //references
    [Header("References")]
    [SerializeField] private Rigidbody2D rb; //reference to the Rigidbody2D component
    [SerializeField] private PlayerJump jump;
    private Controls controls; //reference to the controls


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

    private void FixedUpdate() //run every fixed frame update to ensure physics is consistent (framerate does not affect physics calcs)
    {
        slopeAllowed = SlopeCheck();

        if (Mathf.Abs(moveInput) > 0) //if user is using horizontal inputs
        {
            if (jump.Grounded && !slopeAllowed) //if SlopeCheck() allows walking
            {
                Decelerate();
       //         decelerating = true;
            }
        }
        else //stopped inputting
        {
     //       Decelerate();
        }

        rb.velocity = new Vector2(currSpeedMult * maxSpeed, rb.velocity.y);
    }

    private void StartWalk(InputAction.CallbackContext _ctx)
    {
        //moveInput = _ctx.ReadValue<float>(); //read axis value from the ctx
        moveInput += (_ctx.ReadValue<float>() * 2f - 1f) * Time.deltaTime;
        moveInput = Mathf.Clamp01(moveInput);

    }

    private void CancelWalk()
    {
        moveInput = 0;
    }

    private void Decelerate()
    {
        if (currSpeedMult > 0)
        {
            currSpeedMult = Mathf.Lerp(1, 0, deceleration * Time.deltaTime);
        }
        else
        {
            currSpeedMult = 0;
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
        else //if not both hit
        {
            slopeAngle = hitLeft ? angleLeft : angleRight; //returns the angle greater than 0
        }

        if (Mathf.Abs(slopeAngle) > maxSlopeAngle) //if slope is greater than max slope angle
        {
            //return true if NOT (slope is right AND moving right) OR (slope left AND moving left)
            return !((slopeAngle > 0 && moveInput > 0) || (slopeAngle < 0 && moveInput < 0)); 
        }

        return true; //slope is shallower than max angle
    }

    private void OnDrawGizmosSelected() //runs when game object selected in editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftRayPos.position, leftRayPos.position + Vector3.down * rayDist); //draw left slope ray
        Gizmos.DrawLine(rightRayPos.position, rightRayPos.position + Vector3.down * rayDist); //draw right slope ray

        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, slopeAngle) * Vector2.right);
    }

}
