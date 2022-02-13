using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private Transform checkPos;
    [SerializeField] private LayerMask crateLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform carryCratePos;
    [SerializeField] private Transform carryKeyPos;

    private GameObject keyToCarry;
    private Crate crate;
    
    private bool isPulling;
    public bool IsPulling
    {
        get { return isPulling; }
    }

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();

        controls.Player.Pull.performed += _ => Pull();
        controls.Player.Pull.canceled += _ => StopPull();

        controls.Player.Interact.performed += _ => Interact();
    }

    private void OnEnable()
    {
        controls.Player.Pull.Enable();
        controls.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Pull.Disable();
        controls.Player.Interact.Disable();
    }

    private void Pull()
    {
        if (!isPulling)
        {
            Collider2D crateCol = Physics2D.OverlapBox(checkPos.position, checkSize, 0f, crateLayer);

            if (crateCol != null)
            {
                if (crateCol.CompareTag("Crate"))
                {
                    crate = crateCol.GetComponent<Crate>();
                    crate.StartPull(transform, carryCratePos.localPosition);
                
                    isPulling = true;
                }
            }
        }
    }

    public void StopPull()
    {
        if (isPulling)
        {
            if (crate != null)
            {
                crate.StopPull();
                crate = null;
            }

            isPulling = false;
        }
    }

    private void Interact()
    {
        Collider2D interactCol = Physics2D.OverlapBox(checkPos.position, checkSize, 0f, interactableLayer);
        
        if (interactCol != null)
        {
            if (interactCol.CompareTag("Locked Door"))
            {
                Activatable lockedDoor = interactCol.GetComponent<Activatable>();

                if (lockedDoor != null && keyToCarry != null)
                {
                    lockedDoor.ActivatedKey();
                    Destroy(keyToCarry);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Key"))
        {
            if (keyToCarry == null)
            {
                keyToCarry = collision.gameObject;

                keyToCarry.transform.SetParent(transform);
                keyToCarry.GetComponent<BoxCollider2D>().enabled = false;
                keyToCarry.GetComponent<Animator>().SetBool("pickedUp", true);
                keyToCarry.transform.localPosition = carryKeyPos.localPosition;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(checkPos.position, checkSize);
    }
}
