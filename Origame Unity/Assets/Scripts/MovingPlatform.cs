using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed;
    public bool oneTimeMovement;

    [SerializeField] private bool loop;
    /*[HideInInspector]*/ public bool goingForwards = true;

    [SerializeField] private GameObject targetPositionsParent;
    private List<Transform> targetPositions = new List<Transform>();
    private int posIndex = 1;

    [SerializeField] private Vector2 carryCheckSize;
    [SerializeField] private Transform carryCheckPos;
    [SerializeField] private LayerMask carryLayers;
    private Collider2D[] carryCols = new Collider2D[10];

    void Start()
    {
        goingForwards = true;
        targetPositionsParent.GetComponentsInChildren(targetPositions);
        targetPositions.RemoveAt(0);
    }

    private void FixedUpdate()
    {
        foreach (Collider2D colToCarry in carryCols)
        {
            if (colToCarry != null)
            {
                colToCarry.transform.SetParent(null);
            }
        }

        carryCols = Physics2D.OverlapBoxAll(carryCheckPos.position, carryCheckSize, transform.eulerAngles.z, carryLayers);

        foreach (Collider2D colToCarry in carryCols)
        {
            if (colToCarry != null)
            {
                if (colToCarry.CompareTag("Crate"))
                {
                    colToCarry.transform.eulerAngles = Vector3.zero;

                    Crate crate = colToCarry.GetComponent<Crate>();
                    
                    if (crate != null)
                    {
                        if (crate.IsPulling)
                        {
                            continue;
                        }
                    }
                }
        
                colToCarry.transform.SetParent(transform);
            }
        }

        if ((targetPositions[posIndex].position - transform.position).sqrMagnitude < 0.02f * 0.02f) //close to end position
        {
            ReachPoint();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPositions[posIndex].position, speed * Time.deltaTime); //move towards end
        }
    }

    public void ReachPoint()
    {
        if (goingForwards)
        {
            posIndex++;

            if (posIndex >= targetPositions.Count)
            {
                if (!oneTimeMovement)
                {
                    if (loop)
                    {
                        posIndex = 0;
                    }
                    else 
                    {
                        goingForwards = false;
                        posIndex--;
                    }
                }
                else
                {
                    posIndex--;
                }
            }
        }
        else
        {
            posIndex--;

            if (posIndex < 0)
            {
                if (!oneTimeMovement)
                {
                    posIndex = targetPositions.Count - 2;
                    goingForwards = true;
                }
                else
                {
                    posIndex++;
                }
            }
        }
    }

    private void OnEnable()
    {
        goingForwards = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(carryCheckPos.position, carryCheckSize);
    }
}
