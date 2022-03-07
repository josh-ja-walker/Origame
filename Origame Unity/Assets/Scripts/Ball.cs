using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) //hit a death trigger
    {
        if (collision.CompareTag("Kill") || collision.CompareTag("Laser"))
        {
            Kill();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //exited an object kill
    {
        if (collision.CompareTag("Object Kill"))
        {
            //Kill();
        }
    }

    private void Kill() //kill the ball
    {
        BallSpawner ballSpawner = transform.parent.GetComponent<BallSpawner>(); //get ball spawner
        ballSpawner.currBalls--; //decrease current number of balls
        ballSpawner.Spawn(); //spawn a new one

        Destroy(gameObject); //destroy this
    }
}
