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
    
    private void Start()
    {
        if (PlayerPrefs.HasKey("SavedPosX"))
        {
            transform.position = new Vector3(PlayerPrefs.GetFloat("SavedPosX"), PlayerPrefs.GetFloat("SavedPosY"));
        }
        else
        {
            transform.position = defaultPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            PlayerPrefs.SetFloat("SavedPosX", collision.transform.parent.GetChild(0).position.x);
            PlayerPrefs.SetFloat("SavedPosY", collision.transform.parent.GetChild(0).position.y);
        }       
        else if (collision.gameObject.CompareTag("Kill") || collision.gameObject.CompareTag("Laser"))
        {
            foreach (MonoBehaviour script in disableWhenDie)
            {
                script.enabled = false;
            }
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            anim.SetTrigger("dead");
            StartCoroutine(LoadAfterDeath());
        }
    }

    public IEnumerator LoadAfterDeath()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(respawnWait);
        
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(defaultPos, 0.5f);
    }
}
