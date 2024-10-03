using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Killable
{
    /* Freeze when in hole */
    public void Freeze() {
        rb.bodyType = RigidbodyType2D.Static;
    }

    /* Kill the ball */
    protected override void Die() { 
        BallSpawner ballSpawner = transform.parent.GetComponent<BallSpawner>(); //get ball spawner
        ballSpawner.KillBall(); //decrease current number of balls
        Destroy(gameObject); //destroy this
    }

}
