using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private Transform checkPos;
    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private LayerMask ballLayer;
    
    private bool activated;
    public bool Activated
    {
        get { return activated; }
    }
    [SerializeField] private Activatable activatable;
    
    private void Update()
    {
        if (!activated && Physics2D.OverlapBox(checkPos.position, checkSize, 0f, ballLayer))
        {
            activatable.ActivatedKey();

            ballSpawner.Spawn();
            
            activated = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(checkPos.position, checkSize);
    }
}
