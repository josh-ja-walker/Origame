using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTrigger : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private Animator fadeAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        while (true)
        {
            fadeAnim.SetBool("fading", true);

            Time.timeScale = 0;


            yield return new WaitForSecondsRealtime(waitTime);


            fadeAnim.SetBool("fading", false);

            if (!GameManager.GM.IsPaused)
            {
                Time.timeScale = 1;
            }

            break;
        }

    }
}
