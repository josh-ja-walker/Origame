using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract instance;

    [Header("Interact")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private Vector2 checkOffset = new Vector2(0.5f, -0.1f);
    
    [Header("Crate")]
    [SerializeField] private LayerMask crateLayer;
    [SerializeField] private Vector2 crateCheckSize;
    [SerializeField] private Vector2 crateCheckOffset = new Vector2(0.375f, -0.1f);
    [SerializeField] private Vector2 carryCrateOffset = new Vector2(0.75f, -0.05f);
    public Vector2 CarryCrateOffset { get => carryCrateOffset; }

    private Crate crate;


    [Header("Key")]
    [SerializeField] private Vector2 carryKeyOffset = new Vector2(0.75f, -0.125f);
    private Activator key;
    

    [Header("References")]
    [SerializeField] private AudioSource keyAudio;
    private FixedJoint2D joint;
    private Rigidbody2D rb;
    public Rigidbody2D Rb { get => rb; }

    private Controls controls;

    private void Awake() {
        if (instance == null) {
            instance = this; //set reference to this
        } else {
            Destroy(gameObject); //otherwise destroy
        }

        controls = new Controls();

        controls.Player.Pull.performed += _ => Pull();
        controls.Player.Pull.canceled += _ => StopPull();

        controls.Player.Interact.performed += _ => Interact();

    }

    private void Start() {
        rb = GetComponentInParent<Rigidbody2D>();
        joint = rb.gameObject.GetComponent<FixedJoint2D>();
        joint.enabled = false;
    }


    private void OnEnable() {
        controls.Player.Pull.Enable();
        controls.Player.Interact.Enable();
    }

    private void OnDisable() {
        controls.Player.Pull.Disable();
        controls.Player.Interact.Disable();
    }

    public bool IsPulling() { 
        return crate != null; 
    }

    private void Pull() {
        if (crate != null) {
            return;
        }
        
        Collider2D crateCol = Physics2D.OverlapBox(Offset.Apply(crateCheckOffset, transform), crateCheckSize, 0f, crateLayer);

        if (crateCol != null && crateCol.CompareTag("Crate")) {
            crate = crateCol.GetComponent<Crate>();

            joint.connectedBody = crate.GetComponent<Rigidbody2D>();
            joint.anchor = Offset.Apply(carryCrateOffset, transform) - (Vector2) transform.position;
            joint.enabled = true;
            
            crate.StartPull();
        }
    }

    public void StopPull() {
        if (crate != null) {
            joint.enabled = false;
            crate.StopPull();
            crate = null;
        }
    }



    private void Interact() {
        Collider2D interactCol = Physics2D.OverlapBox(Offset.Apply(checkOffset, transform), checkSize, 0f, interactableLayer);
        
        if (interactCol != null && interactCol.CompareTag("Locked Door")) {
            Activatable lockedDoor = interactCol.GetComponentInParent<Activatable>();

            if (lockedDoor != null && key != null) {
                key.Activate();
                Destroy(key.gameObject);
                key = null;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Key") && key == null) {
            keyAudio.Play();

            key = collision.GetComponent<Activator>();
            
            key.transform.SetParent(transform);
            key.transform.localPosition = carryKeyOffset;
            
            key.GetComponent<BoxCollider2D>().enabled = false;
            key.GetComponent<Animator>().SetBool("pickedUp", true);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Offset.Apply(checkOffset, transform), checkSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Offset.Apply(crateCheckOffset, transform), crateCheckSize);
        Gizmos.DrawSphere(Offset.Apply(carryCrateOffset, transform), 0.05f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Offset.Apply(carryKeyOffset, transform), 0.05f);
    }
}
