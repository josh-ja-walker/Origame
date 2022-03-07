using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ElevatorMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    private void Update()
    {
        if (transform.root.CompareTag("Player"))
        {
            if (!GameManager.GM.music.isPlaying && GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
                GameManager.GM.music.Play();
            
            audioSource.Pause();
        }
        else
        {
            GameManager.GM.music.Pause();
            
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }
}
