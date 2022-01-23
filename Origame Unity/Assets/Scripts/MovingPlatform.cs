using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform end;
    
    [SerializeField] private bool oneTimeMove;
    
    private bool canMove;
    
    private Vector2 start;
    private Vector2 targetPos;

    void Start()
    {
        start = transform.position;
        targetPos = end.position;
    }

    private void FixedUpdate()
    {
        if ((targetPos - (Vector2)transform.position).sqrMagnitude < 0.02f * 0.02f) //close to end position
        {
            SetNext();
        }
        else 
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime); //move towards end
        }
    }

    private void SetNext()
    {
        if (targetPos == start)
        {
            targetPos = end.position;
        }
        else if (!oneTimeMove)
        {
            targetPos = start;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.position.y < transform.position.y)
        {
            SetNext();
        }
    }
}
