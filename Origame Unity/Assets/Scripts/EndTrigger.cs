using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private Animator credits;
    [SerializeField] private SpriteRenderer playerSprite;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) //if player comes into contact with this
        {
            collision.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; //freeze player
            
            Time.timeScale = 0; //freeze time

            playerSprite.sortingLayerName = "Canvas"; //move player in front of credits
            playerSprite.sortingOrder = 1;

            credits.SetTrigger("start"); //start credits

            GameManager.GM.ending = true;
        }
    }
}
