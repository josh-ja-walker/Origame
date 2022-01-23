using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private bool pressed;

    [SerializeField] private PressurePlate[] linkedPlates;
    [SerializeField] private Activatable activatable;

    [SerializeField] private Transform boxPoint;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask activateLayers;

    [SerializeField] private Animator anim;
            
    private void FixedUpdate()
    {
        if (Physics2D.OverlapBox(boxPoint.position, boxSize, 0f, activateLayers))
        {
            if (!pressed)
            {
                try
                {
                    activatable.ActivatedKey();
                }
                catch (System.NullReferenceException) { }
            
                pressed = true;
                anim.SetBool("pressed", true);
            }
        }
        else if (pressed)
        {
            try
            {
                activatable.DeactivatedKey();
            }
            catch (System.NullReferenceException) { }

            pressed = false;
            anim.SetBool("pressed", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxPoint.position, boxSize);
    }
}
