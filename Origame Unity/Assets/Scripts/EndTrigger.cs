using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private Animator credits;
    [SerializeField] private SpriteRenderer playerSprite;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            
            Time.timeScale = 0;

            playerSprite.sortingLayerName = "Canvas";
            playerSprite.sortingOrder = 1;

            credits.SetTrigger("start");

            GameManager.GM.ending = true;
        }
    }
}
