using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool interruptMusic = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interruptMusic) { GameManager.GM.music.Pause(); }
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interruptMusic) { GameManager.GM.music.Play(); }

            audioSource.Pause();
        }
    }

}
