using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private Transform checkPos;
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private LayerMask ballLayer;
    
    private bool activated;
    public bool Activated
    {
        get { return activated; }
    }

    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private Key key;

    private void Update()
    {
        Collider2D ballCol = Physics2D.OverlapBox(checkPos.position, checkSize, 0f, ballLayer); //check for a ball collider

        if (ballCol != null) //if its not null
        {
            if (!activated) //if not already activated
            {
                Debug.Log("activate");
                key.Activate();
                ballSpawner.Spawn();

                activated = true;
            }
        }
        else if (activated) //if no ball collider got, deactivate
        {
            key.Deactivate();
            
            activated = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(checkPos.position, checkSize);
    }
}
