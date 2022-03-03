using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager GM; //static reference

    [Header("References")]
    public GameObject player; //reference
    public PlayerFold playerFold; //reference
    public PlayerSpawn playerSpawn; //reference
    public CinemachineBrain cBrain; //reference
    public AudioSource music;
    public AudioMixerSnapshot play;
    public AudioMixerSnapshot pause;

    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject border;
    
    [SerializeField] private GameObject startScreen;
    private bool returningToStart;
    
    [SerializeField] private GameObject pauseScreen;
    private bool isPaused;
    public bool IsPaused
    {
        get { return isPaused; }
    }
    
    [SerializeField] private GameObject optionsScreen;
    public GameObject OptionsScreen
    {
        get { return optionsScreen; }
    }

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private float loadingExtraTime;


    private Controls controls;

    private void Awake()
    {
        controls = new Controls();

        if (GM == null)
        {
            GM = this; //set reference to this
            DontDestroyOnLoad(gameObject); //make persistent across scenes
        }
        else
        {
            Destroy(gameObject);
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
        if (playerFold != null) 
        {
            playerFold.enabled = !isPaused; //if paused, cannot fold
        }
    }

    private void EscPressed() //pressed the esc button
    {
        if (isPaused) //if already paused
        {
            Resume(); //resume the game
        }
        else if (SceneManager.GetActiveScene().buildIndex != 0) //if not already paused and active scene is not main menu
        {
            Pause(); //pause the game
        }
    }

    public void Pause()
    {
        pause.TransitionTo(0.25f);

        pauseScreen.SetActive(true); //show pause screen
        border.SetActive(true); //turn on border

        cBrain.m_IgnoreTimeScale = false; //freezes camera

        Time.timeScale = 0f; //timescale is 0 - means game is not running anything (unless runs on realtime)

        isPaused = true;
    }

    public void Resume()
    {
        play.TransitionTo(0.25f);

        pauseScreen.SetActive(false); //turn off pause screen
        border.SetActive(false); //turn off border
        
        optionsScreen.SetActive(false); //turn off the options screen (in case esc pressed when on options)

        cBrain.m_IgnoreTimeScale = true; //unfreezes cam

        Time.timeScale = 1f; //normal timescale

        isPaused = false;
    }

    public void HandleOptionsBack() //in case options back is pressed
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) //if current scene is main menu
        {
            startScreen.SetActive(true); //going back activates start menu
        }
        else
        {
            pauseScreen.SetActive(true); //going back activates pause screen
            pause.TransitionTo(0.25f);
        }
    }

    public void LoadLevel(int buildIndex) //load a level, called by button
    {
        loadingScreen.SetActive(true); //turn on load screen
        StartCoroutine(LoadAsync(buildIndex)); //load
    }

    IEnumerator LoadAsync(int buildIndex) //use IEnumerator, allows for while loops and using WaitForSecondsRealtime()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(loadingExtraTime); //wait loadtime, using realtime to account for timescale = 0
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex); //load scene asynchronously
            
            while (!operation.isDone) //while game is not finished
            {
                Debug.Log("Loading");
                yield return null; 
            }

            break;
        }

        loadingScreen.SetActive(false); //turn off load screen
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) //called when new scene loaded
    {
        if (music != null)
        {
            if (!music.isPlaying)
            {
                music.Play(); //play music if not already playing and not null
            }
        }

        Time.timeScale = 1; 

        if (scene.buildIndex != 0) //if scene loaded is not main menu
        {
            foreach (GameObject gameObject in scene.GetRootGameObjects())
            {
                if (gameObject.CompareTag("Player")) //find player for reference
                {
                    player = gameObject; 
                }
            }

            if (player != null)
            {
                //get scripts for referencing
                playerFold = player.GetComponentInChildren<PlayerFold>();
                playerSpawn = player.GetComponent<PlayerSpawn>();
            }

            cBrain = Camera.main.GetComponent<CinemachineBrain>(); //find camera brain for referencing
            
            if (border != null)
            {
                border.SetActive(false); //turn off border
            }
        }
        else if (returningToStart) //if loaded main menu and returning from level
        {
            if (border != null)
            {
                border.SetActive(true); //turn on border
            }

            startScreen = GameObject.Find("Start Screen"); //find the start screen
        }

        Camera main = Camera.main; 

        if (main != null && canvas != null)
        {
            canvas.worldCamera = main; //set canvas camera to main camera
        }
    }

    public void Return() //called when return button pressed
    {
        returningToStart = true;
        Resume();
    }
}