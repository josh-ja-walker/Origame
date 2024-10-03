using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private bool isPulled;

    [Header("Ground Check")]
    [SerializeField] private float offset;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;

    private Vector2 startPos;

    private Rigidbody2D rb;


    private void Start() {
        startPos = transform.position; //set the position to respawn at

        rb = GetComponent<Rigidbody2D>(); //get rigidbody reference
    }

    private void FixedUpdate() {
        bool isGrounded = Physics2D.OverlapBox(transform.position + Vector3.down * offset, groundCheckSize, 0f, groundLayer);

        //if pulling and if crate is not grounded
        if (isPulled && !isGrounded) {
            StopPull();
        }
    }

    public void StartPull() { //start the pulling 
        transform.localEulerAngles = Vector3.zero;
        isPulled = true;
    }

    public void StopPull() { //stop the pull
        isPulled = false;
    }

    public bool IsPulled() { return isPulled; } 

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Kill")) {
            Respawn();
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Object Kill")) {
            Respawn();
        }
    }

    private void Respawn() { //kill the box and respawn it
        StopPull();
        rb.velocity = Vector2.zero;
        transform.eulerAngles = Vector3.zero;
        transform.position = startPos;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.down * offset, groundCheckSize);
    }
}
