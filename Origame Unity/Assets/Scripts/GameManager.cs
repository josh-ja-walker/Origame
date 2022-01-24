using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager GM; //static reference

    [Header("References")]
    public GameObject player; //reference
    public Rigidbody2D playerRB; //reference
    public PlayerFold playerFold; //reference

    [Header("UI")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject loadingScreen;
    private bool isPaused;

    private Controls controls;

    private void Awake() //makes this a singleton
    {
        if (GM != null)
        {
            Destroy(gameObject); //destroy this if already an instance in scene
        }
        else //otherwise
        {
            GM = this; //set reference to this
            DontDestroyOnLoad(gameObject); //make persistent across scenes
        }

        controls = new Controls();

        controls.Player.Pause.performed += _ => EscPressed();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void EscPressed()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        pauseScreen.SetActive(true);

        Time.timeScale = 0f;

        isPaused = true;
    }

    public void Resume()
    {
        pauseScreen.SetActive(false);
        optionsScreen.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
    }

    public void HandleOptionsBack()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            startScreen.SetActive(true);
        }
        else
        {
            pauseScreen.SetActive(true);
        }
    }

    public void LoadLevel(int buildIndex)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadAsync(buildIndex));
    }

    IEnumerator LoadAsync(int buildIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);

        while (!operation.isDone)
        {
            Debug.Log("Loading");
            yield return null;
        }

        loadingScreen.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerRB = player.GetComponent<Rigidbody2D>();
            playerFold = player.GetComponentInChildren<PlayerFold>();
        }
    }
}
