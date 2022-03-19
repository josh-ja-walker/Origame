using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private bool isPulling;
    public bool IsPulling
    {
        get { return isPulling; }
    } 

    [SerializeField] private PhysicsMaterial2D crateMat;
    [SerializeField] private float gravityScale = 1;
    [SerializeField] private float mass = 5;

    [SerializeField] private float offset;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;

    private Vector2 startPos;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float respawnTime;

    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        startPos = transform.position; //set the position to respawn at
    }

    private void FixedUpdate()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            //set up rigidbody's values
            rb.gravityScale = gravityScale;
            rb.mass = mass;
            rb.sharedMaterial = crateMat;
        }

        //if pulling and if crate is not grounded
        if (isPulling && !Physics2D.OverlapBox(transform.position + Vector3.down * offset, groundCheckSize, 0f, groundLayer))
        {
            StopPull();
        }
    }

    public void StartPull(Transform holdPos, Vector2 offset) //start the pulling
    {
        transform.SetParent(holdPos); //set the parent to the player
        transform.localPosition = offset; //offset the box
        transform.localEulerAngles = Vector3.zero; //reset the box's rotation

        Destroy(rb); //destroy the rigidbody to allow the box to be moved as a child

        isPulling = true;
    }

    public void StopPull() //stop the pull
    {
        transform.SetParent(null); //remove the parent
        rb = gameObject.AddComponent<Rigidbody2D>(); //add a rigidbody component

        isPulling = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kill"))
        {
            Respawn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object Kill"))
        {
            Respawn();
        }
    }

    private void Respawn() //kill the box and respawn it
    {
        rb.velocity = Vector2.zero;
        transform.eulerAngles = Vector3.zero;
        transform.position = startPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.down * offset, groundCheckSize);
    }
}
