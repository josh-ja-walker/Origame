using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proximity : MonoBehaviour
{
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private Transform checkPos;
    [SerializeField] private LayerMask activateLayer;

    [SerializeField] private Key key;

    void FixedUpdate()
    {
        if (Physics2D.OverlapBox(checkPos.position, checkSize, 0f, activateLayer))
        {
            key.Activate();
        }
        else
        {
            key.Deactivate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(checkPos.position, checkSize);
    }
}
