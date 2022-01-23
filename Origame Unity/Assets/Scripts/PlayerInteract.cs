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

    private Crate crate;

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
        Collider2D crateCol = Physics2D.OverlapBox(checkPos.position, checkSize, 0f, crateLayer);

        if (crateCol != null)
        {
            if (crateCol.CompareTag("Crate"))
            {
                crate = crateCol.GetComponent<Crate>();
                crate.StartPull();
            }
        }
    }

    private void StopPull()
    {
        if (crate != null)
        {
            crate.StopPull();
            crate = null;
        }
    }

    private void Interact()
    {
        Collider2D interactCol = Physics2D.OverlapBox(checkPos.position, checkSize, 0f, interactableLayer);
        
        if (interactCol != null)
        {
            if (interactCol.CompareTag("Switch"))
            {
                Switch _switch = interactCol.GetComponent<Switch>();

                if (_switch != null)
                {
                    _switch.Interact();
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(checkPos.position, checkSize);
    }
}
