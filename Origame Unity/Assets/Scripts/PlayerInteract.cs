using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private Vector2 checkOffset = new Vector2(0.5f, -0.1f);
    
    [Header("Crate")]
    [SerializeField] private LayerMask crateLayer;
    [SerializeField] private Vector2 crateCheckSize;
    [SerializeField] private Vector2 crateCheckOffset = new Vector2(0.375f, -0.1f);
    [SerializeField] private Vector2 carryCrateOffset = new Vector2(0.75f, -0.05f);
    private Crate crate;

    private bool isPulling;
    public bool IsPulling {
        get { return isPulling; }
    }
    
    [Header("Key")]
    [SerializeField] private Vector2 carryKeyOffset = new Vector2(0.75f, -0.125f);
    private Activator key;
    

    [Header("References")]
    [SerializeField] private AudioSource keyAudio;

    private Controls controls;

    private void Awake() {
        controls = new Controls();

        controls.Player.Pull.performed += _ => Pull();
        controls.Player.Pull.canceled += _ => StopPull();

        controls.Player.Interact.performed += _ => Interact();
    }

    private void OnEnable() {
        controls.Player.Pull.Enable();
        controls.Player.Interact.Enable();
    }

    private void OnDisable() {
        controls.Player.Pull.Disable();
        controls.Player.Interact.Disable();
    }

    private void Pull() {
        if (!isPulling) {
            Collider2D crateCol = Physics2D.OverlapBox((Vector2) transform.position + checkOffset, crateCheckSize, 0f, crateLayer);

            if (crateCol != null) {
                if (crateCol.CompareTag("Crate")) {
                    crate = crateCol.GetComponent<Crate>();
                    crate.StartPull(transform, (Vector2) transform.position + carryCrateOffset);
                
                    isPulling = true;
                }
            }
        }
    }

    public void StopPull() {
        if (isPulling) {
            if (crate != null) {
                crate.StopPull();
                crate = null;
            }

            isPulling = false;
        }
    }

    private void Interact() {
        Collider2D interactCol = Physics2D.OverlapBox((Vector2) transform.position + checkOffset, checkSize, 0f, interactableLayer);
        
        if (interactCol != null) {
            if (interactCol.CompareTag("Locked Door")) {
                Activatable lockedDoor = interactCol.GetComponent<Activatable>();

                if (lockedDoor != null && key != null) {
                    keyAudio.Play();
                    key.Activate();
                    lockedDoor.Updated(key);
                    Destroy(key.gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Key")) {
            if (key == null) {
                keyAudio.Play();

                key = collision.GetComponent<Activator>();
                
                key.transform.SetParent(transform);
                key.transform.localPosition = carryKeyOffset;
                
                key.GetComponent<BoxCollider2D>().enabled = false;
                key.GetComponent<Animator>().SetBool("pickedUp", true);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + (Vector3) checkOffset, checkSize);
        Gizmos.DrawWireCube(transform.position + (Vector3) carryCrateOffset, crateCheckSize);
    }
}
