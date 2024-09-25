using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    /* Hit a death trigger */
    private void OnTriggerEnter2D(Collider2D collision) { 
        if (collision.CompareTag("Kill") || collision.CompareTag("Laser")) {
            Kill();
        }
    }

    /* Kill the ball */
    private void Kill() { 
        BallSpawner ballSpawner = transform.parent.GetComponent<BallSpawner>(); //get ball spawner
        ballSpawner.KillBall(); //decrease current number of balls
        Destroy(gameObject); //destroy this
    }
}
