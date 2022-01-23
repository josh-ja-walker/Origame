using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private bool pull;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float separateDist;
    [SerializeField] private PhysicsMaterial2D crateMat;
    [SerializeField] private PhysicsMaterial2D playerMat;
    [SerializeField] private LayerMask playerLayer;

    private void FixedUpdate()
    {
        if (pull)
        {
            rb.velocity = new Vector2(GameManager.GM.player.GetComponent<Rigidbody2D>().velocity.x, rb.velocity.y);
        
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector2.one * separateDist);
    }
}
