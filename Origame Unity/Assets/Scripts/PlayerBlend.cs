using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerBlend : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Blend Trigger"))
        {
            collision.gameObject.GetComponent<VCamReference>().VCam.MoveToTopOfPrioritySubqueue();
        }
    }
}
