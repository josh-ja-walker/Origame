using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityActivator : Activator
{
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private Vector2 checkOffset;
    [SerializeField] private LayerMask activateLayer;

    void FixedUpdate() {
        if (Physics2D.OverlapBox(transform.position + (Vector3) checkOffset, checkSize, 0f, activateLayer)) {
            Activate();
        } else {
            Deactivate();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + (Vector3) checkOffset, checkSize);
    }
}
