using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateByProximity : MonoBehaviour
{
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private Transform checkPos;

    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private MovingPlatform movingPlatform;
    [SerializeField] private LayerMask activateLayer;

    void FixedUpdate()
    {
        if (Physics2D.OverlapBox(checkPos.position, checkSize, 0f, activateLayer))
        {
            EnableOrDisable(true);
        }
        else
        {
            EnableOrDisable(false);
        }
    }

    private void EnableOrDisable(bool isEnable)
    {
        if (ballSpawner != null)
        {
            ballSpawner.enabled = isEnable;
        }
        else if (movingPlatform != null)
        {
            movingPlatform.enabled = isEnable;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(checkPos.position, checkSize);
    }
}
