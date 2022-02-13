using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kill") || collision.CompareTag("Laser"))
        {
            Kill();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object Kill"))
        {
            Kill();
        }
    }

    private void Kill()
    {
        BallSpawner ballSpawner = transform.parent.GetComponent<BallSpawner>();
        ballSpawner.currBalls--;
        ballSpawner.Spawn();

        Destroy(gameObject);
    }
}
