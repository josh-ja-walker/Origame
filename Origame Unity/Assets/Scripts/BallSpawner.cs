using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : Activatable
{
    [SerializeField] private readonly int maxBalls; //maximum number of balls allowed in scene
    private int currBalls = 0; //current number of balls

    [SerializeField] private readonly float spawnForce;
    [SerializeField] private readonly float spawnTime;

    [SerializeField] private readonly Transform spawnPos;
    
    [SerializeField] private readonly GameObject ballPrefab;

    [SerializeField] private readonly AudioSource audioSource;


    void Start() {
        StartCoroutine(Run());
    }

    private void Spawn() {
        if (currBalls < maxBalls) {
            GameObject ball = Instantiate(ballPrefab, spawnPos.position, Quaternion.identity, transform);
            ball.GetComponent<Rigidbody2D>().velocity = -transform.up * spawnForce;

            currBalls++;
            if (audioSource != null) {
                audioSource.Play();
            }
        }
    }

    private IEnumerator Run() {
        while (true) {
            if (!IsActive()) {
                yield return null;
            }

            Spawn();
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void KillBall() {
        currBalls--;
    }

}
