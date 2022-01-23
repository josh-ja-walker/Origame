using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    private Vector2 startPos;

    [SerializeField] private Rigidbody2D rb;
    private float gravityScale;
    [SerializeField] private float respawnTime;

    private void Start()
    {
        startPos = transform.position;
        gravityScale = rb.gravityScale;
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
        if (CompareTag("Ball"))
        {
            BallSpawner ballSpawner = transform.parent.GetComponent<BallSpawner>();
            ballSpawner.currBalls--;
            ballSpawner.Spawn();

            Destroy(gameObject);
        }

        rb.velocity = Vector2.zero;
        rb.gravityScale = gravityScale;

        transform.eulerAngles = Vector3.zero;
        transform.position = startPos;
    }
}
