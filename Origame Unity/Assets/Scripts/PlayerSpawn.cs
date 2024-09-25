using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Vector2 defaultPos;
    [SerializeField] private float respawnWait;

    [SerializeField] private Animator anim;

    [SerializeField] private MonoBehaviour[] disableWhenDie;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private AudioSource deathAudio;
    [SerializeField] private AudioSource checkpointAudio;

    private void Start() {
        if (PlayerPrefs.HasKey("SavedPosX")) { //checks if there is a saved position in PlayerPrefs (Unity built-in system for simple saving)
            transform.position = new Vector3(PlayerPrefs.GetFloat("SavedPosX"), PlayerPrefs.GetFloat("SavedPosY")); //move the player there
        } else { //no saved position
            transform.position = defaultPos; //move the player to the default start position
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Checkpoint")) { //walks into a checkpoint trigger
            Vector2 pos = collision.transform.GetChild(0).position;
            
            //if checkpoint not already saved
            if (pos.x != PlayerPrefs.GetFloat("SavedPosX") || pos.y != PlayerPrefs.GetFloat("SavedPosY")) { 
                checkpointAudio.Play();
                PlayerPrefs.SetFloat("SavedPosX", pos.x);
                PlayerPrefs.SetFloat("SavedPosY", pos.y);
            }
            
        } else if (collision.gameObject.CompareTag("Kill") || collision.gameObject.CompareTag("Laser")) { //if dies
            foreach (MonoBehaviour script in disableWhenDie) { //disable scripts
                script.enabled = false;
            }

            rb.bodyType = RigidbodyType2D.Static; //make player a static rigidbody (stop moving)
            anim.SetTrigger("dead"); //play death animation
            GameManager.GM.SendMessage("Fade");

            deathAudio.Play(); //play death audio
            GameManager.GM.music.Pause(); //pause music for death audio

            StartCoroutine(LoadAfterDeath());
        }
    }

    public IEnumerator LoadAfterDeath() { //reload the scene after dying
        while (true) {
            yield return new WaitForSecondsRealtime(respawnWait); //wait some time in realtime (ignores timeScale)
            GameManager.GM.LoadLevel(SceneManager.GetActiveScene().buildIndex);
            break;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(defaultPos, 0.5f);
    }
}
