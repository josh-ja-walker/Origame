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
    [SerializeField] private float loadingExtraTime;
    private bool isPaused;

    public bool IsPaused
    {
        get { return isPaused; }
    }

    private Controls controls;

    private void Awake() //makes this a singleton
    {
        controls = new Controls();

        if (GM != null) //if not the only game manager in scene
        {
            Destroy(gameObject); //destroy this
        }
        else //otherwise
        {
            GM = this; //set reference to this
            DontDestroyOnLoad(gameObject); //make persistent across scenes
        }

        controls.Player.Pause.performed += _ => EscPressed();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        controls.Player.Pause.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Pause.Disable();
    }

    private void Update()
    {
        playerFold.enabled = !isPaused;
    }

    private void EscPressed()
    {
        if (isPaused)
        {
            Resume();
        }
        else if (SceneManager.GetActiveScene().buildIndex != 0)
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

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator LoadAsync(int buildIndex)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(loadingExtraTime);
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
            
            while (!operation.isDone)
            {
                Debug.Log("Loading");
                yield return null;
            }

            break;
        }

        loadingScreen.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1;

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerRB = player.GetComponent<Rigidbody2D>();
            playerFold = player.GetComponentInChildren<PlayerFold>();
        }
    }
}
