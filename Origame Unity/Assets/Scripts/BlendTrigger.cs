using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class BlendTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineBrain cBrain;
    [SerializeField] private CinemachineVirtualCamera vcam;
    
    private void OnTriggerEnter2D(Collider2D collision) //collides with blend trigger
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            vcam.Priority++; //increase new priority

            if (cBrain.ActiveVirtualCamera != null)
            {
                cBrain.ActiveVirtualCamera.Priority--; //decrease old priority
            }
        }
    }
}
