using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private int maxBalls;
    [HideInInspector] public int currBalls = 0;
    
    [SerializeField] private float spawnForce;

    [SerializeField] private float spawnTime;
    [SerializeField] private Transform spawnPos;
    
    [SerializeField] private GameObject ballPrefab;

    private void OnEnable()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (currBalls < maxBalls)
        {
            currBalls++;
            Invoke("InstantiateBall", spawnTime);
        }
    }

    private void InstantiateBall()
    {
        Instantiate(ballPrefab, spawnPos.position, Quaternion.identity, transform).GetComponent<Rigidbody2D>().velocity = -transform.up * spawnForce;
    }
}
