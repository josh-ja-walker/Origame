using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerBlend : MonoBehaviour
{
    [SerializeField] private CinemachineBrain cBrain;

    private void OnTriggerEnter2D(Collider2D collision) //collides with blend trigger
    {
        if (collision.gameObject.CompareTag("Blend Trigger"))
        {
            collision.gameObject.GetComponent<VCamReference>().VCam.Priority++; //increase new priority
            
            if (cBrain.ActiveVirtualCamera != null)
            {
                cBrain.ActiveVirtualCamera.Priority--; //decrease old priority
            }
        } 
    }
}