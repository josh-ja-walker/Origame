using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : Killable
{
    public static PlayerSpawn instance;

    [SerializeField] private Vector2 startPos;

    [SerializeField] private MonoBehaviour[] disableWhenDie;


    [SerializeField] private AudioSource deathAudio;
    [SerializeField] private AudioSource checkpointAudio;

    [SerializeField] private float fadeCooldownFactor = 0.8f; 

    private bool isDead = false;

    private void Awake() {
        if (instance == null) {
            instance = this; //set reference to this
        } else {
            Destroy(gameObject); //otherwise destroy
        }
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        if (PlayerPrefs.HasKey("SavedPosX")) { //checks if there is a saved position in PlayerPrefs (Unity built-in system for simple saving)
            transform.position = new Vector3(PlayerPrefs.GetFloat("SavedPosX"), PlayerPrefs.GetFloat("SavedPosY")); //move the player there
        } else { //no saved position
            transform.position = startPos; //move the player to the default start position
        }
    
    }


    private void OnTriggerEnter2D(Collider2D col) {
        if (IsKillObject(col.gameObject)) {
            DieAndRespawn();
        } else if (col.gameObject.CompareTag("Checkpoint")) { //walks into a checkpoint trigger
            for (int i = 0; i <= col.transform.GetSiblingIndex() && i < col.transform.parent.childCount; i++) {
                Animator checkpointAnim = col.transform.parent.GetChild(i).GetComponent<Animator>();
                if (checkpointAnim != null) {
                    checkpointAnim.SetTrigger("reached");
                }
            }
        
            Vector2 pos = col.transform.GetChild(0).position;
            
            //if checkpoint not already saved
            if (pos.x != PlayerPrefs.GetFloat("SavedPosX") || pos.y != PlayerPrefs.GetFloat("SavedPosY")) { 
                checkpointAudio.Play();
                PlayerPrefs.SetFloat("SavedPosX", pos.x);
                PlayerPrefs.SetFloat("SavedPosY", pos.y);
            }

        }
    }
    

    public bool IsDead() {
        return isDead;
    }

    protected override void Die() {
        isDead = true;

        foreach (MonoBehaviour script in disableWhenDie) { //disable scripts
            script.enabled = false;
        }
        
        rb.bodyType = RigidbodyType2D.Static; //make player a static rigidbody (stop moving)
        
        anim.SetTrigger("dead"); //play death animation
        deathAudio.Play(); //play death audio

        if (GameManager.GM != null) {
            GameManager.GM.Invoke("FadeIn", respawnCooldown * fadeCooldownFactor);
            GameManager.GM.music.Pause(); //pause music for death audio
        }
    }
    
    protected override void Respawn() {
        isDead = false;

        int curScene = SceneManager.GetActiveScene().buildIndex;
        if (GameManager.GM != null) {
            GameManager.GM.LoadLevel(curScene);
        } else {
            SceneManager.LoadScene(curScene);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPos, 0.5f);
    }
}
