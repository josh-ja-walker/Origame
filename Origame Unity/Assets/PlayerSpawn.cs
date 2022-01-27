using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Vector2 defaultPos;
    [SerializeField] private float respawnWait;
    
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
        else if (collision.gameObject.CompareTag("Kill"))
        {
            Time.timeScale = 0f;
            StartCoroutine(LoadAfterDeath());
        }
    }

    IEnumerator LoadAfterDeath()
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
