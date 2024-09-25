using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    private Vector2 startPos;
    private float gravityScale;

    [SerializeField] private readonly float respawnTime;
    
    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        gravityScale = rb.gravityScale;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Kill")) {
            rb.gravityScale = 0;
            Invoke(nameof(Respawn), respawnTime);
        }
    }

    private void Respawn() {
        rb.velocity = Vector2.zero;
        rb.gravityScale = gravityScale;

        transform.eulerAngles = Vector3.zero;
        transform.position = startPos;
    }
}
