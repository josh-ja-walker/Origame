using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class MovingPlatform : Activatable
{

    [Header("Movement Settings")]
    [SerializeField] private float speed = 3;
    [SerializeField] private bool pauseAtPositions = true;
    [SerializeField] private float pauseTime = 0.5f;
    [SerializeField] private bool toggleDirectionOnActivate;

    [SerializeField] private MovementType movementType = MovementType.Loop;
    enum MovementType {
        Loop,
        PingPong,
        StayAtEnd
    }
    

    [Header("References")]

    [SerializeField] private GameObject positionsParent;
    private Transform[] positions;
    private int index = 0;
    [SerializeField] private AudioSource elevatorMusic;


    [Header("Carrying")]
    [SerializeField] private Vector2 carryCheckSize;
    [SerializeField] private Vector2 carryCheckOffset;
    [SerializeField] private LayerMask carryLayers;
    private Collider2D[] carryCols = new Collider2D[10];


    private bool movingForwards = true;
    private bool reached = false;
    private static readonly float REACHED_POINT_CHECK = 0.02f;
    


    void Start() {
        positions = positionsParent.GetComponentsInChildren<Transform>()
            .Where(t => t != positionsParent.transform)
            .ToArray();

        if (toggleDirectionOnActivate) {
            movingForwards = IsActive();
        }

        if (!movingForwards) {
            index = positions.Length - 1;
        }
    }

    private void FixedUpdate() {
        if (toggleDirectionOnActivate) {
            movingForwards = IsActive();
        } else if (!IsActive()) {
            return;
        }
        
        /* Check if reached position */ 
        if (ReachedPoint() && !reached) { //close to end position
            reached = true;
            transform.position = TargetPosition();
            Invoke(nameof(NextPoint), pauseAtPositions ? pauseTime: 0);
        } else {
            transform.position = Vector2.MoveTowards(transform.position, TargetPosition(), speed * Time.deltaTime); //move towards end
        }

        //find objects to carry
        Collider2D[] newCarryCols = Physics2D.OverlapBoxAll(
            Offset.Apply(carryCheckOffset, transform), 
            carryCheckSize,
            transform.eulerAngles.z, 
            carryLayers
        );

        foreach (Collider2D col in carryCols.Where(col => !newCarryCols.Contains(col))) {
            Release(col);
        }

        foreach (Collider2D col in newCarryCols.Where(col => !carryCols.Contains(col))) {
            Carry(col);
        }

        carryCols = newCarryCols;
    }

    private void Carry(Collider2D col) {
        if (col != null) {

            //if player is being carried by moving platform
            if (col.CompareTag("Player")) { 
                if (GameManager.GM != null) {
                    GameManager.GM.music.Pause(); //pause music
                }
                
                if (!elevatorMusic.isPlaying) { //is elevator music is not already playing
                    elevatorMusic.Play(); //play it
                }

            } else if (col.CompareTag("Crate") && col.GetComponent<Crate>().IsPulled()) {
                return; //if crate is being pulled, ignore
            }

            col.transform.SetParent(transform); //carry these objects
        }
    }

    private void Release(Collider2D col) {
        if (col != null) {

            //if player is being released by moving platform
            if (col.CompareTag("Player")) { 
                //if music is not playing and player is not dead
                if (GameManager.GM != null && !GameManager.GM.music.isPlaying && !PlayerSpawn.instance.IsDead()) {
                    GameManager.GM.music.Play(); //play music
                }
                
                elevatorMusic.Stop(); //stop elevator music

            } else if (col.CompareTag("Crate") && col.GetComponent<Crate>().IsPulled()) {
                return; //if crate is being pulled, ignore
            }
            
            col.transform.SetParent(null); 
        }
    }

    private void OnDisable() {
        foreach (Collider2D col in carryCols) {
            Release(col);
        }
    }

    private Vector2 TargetPosition() {
        return positions[movingForwards ? index : positions.Length - index - 1].position;
    }

    private bool ReachedPoint() {
        return (TargetPosition() - (Vector2) transform.position).sqrMagnitude < REACHED_POINT_CHECK * REACHED_POINT_CHECK;
    }

    private void NextPoint() { //reached a waypoint
        reached = false;
        
        if (index < positions.Length - 1) {
            index++; //increase index to move to next position
        } else {
            //if increment would overflow
            switch (movementType) {
                case MovementType.Loop: 
                    index = 0;
                    break;
                case MovementType.PingPong: 
                    movingForwards = !movingForwards;
                    break;
                case MovementType.StayAtEnd: 
                    break;
            };
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        foreach (Transform pos in positionsParent.GetComponentsInChildren<Transform>()) {
            Gizmos.DrawSphere(pos.position, 0.05f);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + (Vector3) carryCheckOffset, carryCheckSize);
    }
}
