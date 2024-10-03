using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ElevatorMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    private void Update() {
        if (transform.root.CompareTag("Player")) { //if player is not being carried by moving platform
            //if music is not playing and player is not dead
            if (GameManager.GM != null && !GameManager.GM.music.isPlaying && GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static) {
                GameManager.GM.music.Play(); //play music
            }
            
            audioSource.Stop(); //stop elevator music
        } else { //player is being carried by moving platform
            if (GameManager.GM != null) {
                GameManager.GM.music.Pause(); //pause music
            }
            
            if (!audioSource.isPlaying) { //is elevator music is not already playing
                audioSource.Play(); //play it
            }
        }
    }
}
