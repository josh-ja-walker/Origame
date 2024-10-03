using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : Activatable
{
    [SerializeField] private int maxBalls; //maximum number of balls allowed in scene
    private int currBalls = 0; //current number of balls

    [SerializeField] private float spawnForce;
    [SerializeField] private float spawnTime;

    [SerializeField] private Vector2 spawnOffset;
    
    [SerializeField] private GameObject ballPrefab;

    [SerializeField] private AudioSource audioSource;


    void Start() {
        StartCoroutine(Run());
    }

    private void Spawn() {
        if (currBalls < maxBalls) {
            GameObject ball = Instantiate(ballPrefab, Offset.Apply(spawnOffset, transform), Quaternion.identity, transform);
            ball.GetComponent<Rigidbody2D>().velocity = -transform.up * spawnForce;

            currBalls++;
            
            if (audioSource != null) {
                audioSource.Play();
            }
        }
    }

    private IEnumerator Run() {
        while (true) {
            while (!IsActive()) {
                yield return null;
            }

            Spawn();
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void KillBall() {
        currBalls--;
    }

    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Offset.Apply(spawnOffset, transform), 0.05f);
    }

}
