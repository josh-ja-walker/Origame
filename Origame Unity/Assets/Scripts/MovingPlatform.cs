using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float waitTime = 0.25f;
    public bool oneTimeMovement;

    [SerializeField] private bool loop;
    [HideInInspector] public bool goingForwards = true;

    [SerializeField] private GameObject targetPositionsParent;
    private List<Transform> targetPositions = new List<Transform>();
    private int posIndex = 1;

    [SerializeField] private Vector2 carryCheckSize;
    [SerializeField] private Transform carryCheckPos;
    [SerializeField] private LayerMask carryLayers;
    private Collider2D[] carryCols = new Collider2D[10];

    [SerializeField] private SpriteRenderer chainSprite;
    [SerializeField] private Transform chainTopPoint;
    [SerializeField] private Transform chainStartPoint;

    public bool isMoving;

    void Start()
    {
        goingForwards = true;
        targetPositionsParent.GetComponentsInChildren(targetPositions);
        targetPositions.RemoveAt(0);
    }

    private void FixedUpdate()
    {
        chainSprite.size = new Vector2(1f, chainTopPoint.position.y - chainStartPoint.position.y);
        chainSprite.transform.localPosition = new Vector2(0, chainStartPoint.localPosition.y + chainSprite.size.y / 2f);

        if (isMoving)
        {
            //loop through objects the platform carries
            foreach (Collider2D colToCarry in carryCols)
            {
                if (colToCarry != null)
                {
                    if (colToCarry.CompareTag("Crate"))
                    {
                        Crate crate = colToCarry.GetComponent<Crate>();

                        if (crate != null)
                        {
                            if (crate.IsPulling)
                            {
                                continue; //if crate is being pulled, ignore
                            }
                        }
                    }

                    colToCarry.transform.SetParent(null); //stop carrying objects
                }
            }

            //find objects to carry
            carryCols = Physics2D.OverlapBoxAll(carryCheckPos.position, carryCheckSize, transform.eulerAngles.z, carryLayers);

            //loop through again
            foreach (Collider2D colToCarry in carryCols)
            {
                if (colToCarry != null)
                {
                    if (colToCarry.CompareTag("Crate"))
                    {
                        colToCarry.transform.eulerAngles = Vector3.zero;

                        Crate crate = colToCarry.GetComponent<Crate>(); //if trying to carry a crate
                    
                        if (crate != null)
                        {
                            if (crate.IsPulling)
                            {
                                continue; //if its being pulled, ignore
                            }
                        }
                    }
        
                    colToCarry.transform.SetParent(transform); //carry these objects
                }
            }

            if ((targetPositions[posIndex].position - transform.position).sqrMagnitude < 0.02f * 0.02f) //close to end position
            {
                isMoving = false;
                ReachPoint();
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPositions[posIndex].position, speed * Time.deltaTime); //move towards end
            }

        }
    }

    public void ReachPoint() //reached a waypoint
    {
        if (goingForwards) //if moving forwards
        { 
            posIndex++; //increase index to move to next position

            if (posIndex >= targetPositions.Count) //if position is greater than number of positions
            {
                if (!oneTimeMovement) //if not stop now
                {
                    isMoving = true;

                    if (loop)
                    {
                        posIndex = 0; //move towards first index in a loop
                    }
                    else //should reciprocate
                    {
                        goingForwards = false; 
                        posIndex--;

                        ReachPoint();
                    }
                }
            }
        }
        else
        {
            posIndex--; //go backwards, go to previous indexes

            if (posIndex < 0) //if index is out of range
            {
                if (!oneTimeMovement) 
                {
                    isMoving = true;

                    goingForwards = true;
                    posIndex = 1; 
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
