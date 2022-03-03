using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private int maxBalls; //maximum number of balls allowed in scene
    [HideInInspector] public int currBalls = 0; //current number of balls
    
    [SerializeField] private float spawnForce;

    [SerializeField] private float spawnTime;
    [SerializeField] private Transform spawnPos;
    
    [SerializeField] private GameObject ballPrefab;

    [SerializeField] private AudioSource audioSource;

    private void OnEnable()
    {
        Spawn();
    }

    public void Spawn() //handle spawning
    {
        if (currBalls < maxBalls)
        {
            currBalls++;
            
            if (audioSource != null)
            {
                audioSource.Play();
            }

            Invoke("InstantiateBall", spawnTime);
        }
    }

    private void InstantiateBall() //instantiate the ball
    {
        Instantiate(ballPrefab, spawnPos.position, Quaternion.identity, transform).GetComponent<Rigidbody2D>().velocity = -transform.up * spawnForce;
    }

}
