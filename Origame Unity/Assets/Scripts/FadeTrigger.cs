using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTrigger : MonoBehaviour
{
    [SerializeField] private float waitTime = 1.2f;
    [SerializeField] private float timeScale = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    public IEnumerator FadeOut()
    {
        while (true)
        {
            GameManager.GM.StartFade();

            yield return new WaitForSecondsRealtime(0.5f);

            Time.timeScale = timeScale;

            yield return new WaitForSecondsRealtime(waitTime);

            if (!GameManager.GM.IsPaused) {
                Time.timeScale = 1;
            }

            gameObject.SetActive(false);

            GameManager.GM.StopFade();

            break;
        }

    }
}
