using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private bool pull;

    [SerializeField] private float separateDist;
    [SerializeField] private PhysicsMaterial2D crateMat;
    [SerializeField] private PhysicsMaterial2D playerMat;
    [SerializeField] private LayerMask playerLayer;

    private Vector2 startPos;

    [SerializeField] private Rigidbody2D rb;
    private float gravityScale;

    [SerializeField] private float respawnTime;

    private void Start()
    {
        startPos = transform.position;
        gravityScale = rb.gravityScale;
    }


    private void FixedUpdate()
    {
        if (pull)
        {
            rb.velocity = new Vector2(GameManager.GM.playerRB.velocity.x, rb.velocity.y);
            //rb.position = new Vector2(GameManager.GM.player.transform.position.x + 0.6f, rb.position.y);
            if (!Physics2D.OverlapBox(transform.position, Vector2.one * separateDist, transform.eulerAngles.z, playerLayer))
            {
                StopPull();
            }
        }
    }

    public void StartPull()
    {
        rb.sharedMaterial = playerMat;
        pull = true;
    }

    public void StopPull()
    {
        rb.sharedMaterial = crateMat;
        pull = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            rb.sharedMaterial = playerMat;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            rb.sharedMaterial = crateMat;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kill"))
        {
            rb.gravityScale = 0;
            Invoke("Respawn", respawnTime);
        }
    }

    private void Respawn()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = gravityScale;

        transform.eulerAngles = Vector3.zero;
        transform.position = startPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector2.one * separateDist);
    }
}
