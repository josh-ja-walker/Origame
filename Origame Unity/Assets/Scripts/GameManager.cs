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

    public const int MENU_SCENE = 0;
    public const int LEVEL_SCENE = 1;

    [Header("Audio")]
    public AudioSource music;
    [SerializeField] private AudioMixerSnapshot play;
    [SerializeField] private AudioMixerSnapshot pause;

    private PlayerSpawn playerSpawn; //reference
    private CinemachineBrain cBrain; //reference
    private GameObject player; //reference

    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject loadingScreen;
    
    [SerializeField] private float loadingTime = 2f;
    
    public GameObject OptionsScreen { get { return optionsScreen; } }

    
    private bool isPaused;
    public bool IsPaused { get { return isPaused; } }
    
    
    private bool ending;

    private Controls controls;

    private void Awake() {
        controls = new Controls();

        if (GM == null) {
            GM = this; //set reference to this
            DontDestroyOnLoad(gameObject); //make persistent across scenes
        } else {
            Destroy(gameObject);
        }

        controls.Player.Pause.performed += _ => EscPressed();
         
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable() {
        controls.Player.Pause.Enable();
    }

    private void OnDisable() {
        controls.Player.Pause.Disable();
    }

    /* Esc button pressed */
    private void EscPressed() { 
        if (!ending && SceneManager.GetActiveScene().buildIndex != 0) {
            TogglePaused();
        }
    }

    private void TogglePaused() {
        isPaused = !isPaused;

        pauseScreen.SetActive(isPaused); //show pause screen
        Time.timeScale = isPaused ? 0f : 1f; //timescale is 0 - means game is not running anything (unless runs on realtime)
        cBrain.m_IgnoreTimeScale = !isPaused; //freezes camera
        (isPaused ? pause : play).TransitionTo(0f); //transition to the pause snapshot (with low pass filter)

        if (PlayerFold.instance != null) {
            PlayerFold.instance.enabled = !isPaused; //if paused, cannot fold
        }
    }


    /* Pause play */
    public void Pause() {
        isPaused = false;
        TogglePaused();
    }


    /* Resume play */
    public void Resume() {
        isPaused = true;
        TogglePaused();
    }


    /* Options back button pressed */
    public void BackFromOptions() { 
        /* Check if current scene is main menu */
        if (SceneManager.GetActiveScene().buildIndex == MENU_SCENE) { 
            startScreen.SetActive(true); //going back activates start menu
        } else { 
            pauseScreen.SetActive(true); //going back activates pause screen
            pause.TransitionTo(0f);
        }

        optionsScreen.SetActive(false);
    }


    /* Delete the saved position of the player */
    public void ResetSavedPos() {
        PlayerPrefs.DeleteKey("SavedPosX");
        PlayerPrefs.DeleteKey("SavedPosY");
    }

    /* Called when return button pressed */
    public void QuitToMenu() {
        pauseScreen.SetActive(false);
        LoadLevel(MENU_SCENE);
    }

    /* Hit end collider, roll credits */
    public void End() {
        ending = true;
    }


    /* Load a level, called by button */
    public void LoadLevel(int buildIndex) { 
        ending = false;

        loadingScreen.SetActive(true); //turn on load screen
        StartCoroutine(LoadAsync(buildIndex)); //load
    }

    /* Load next scene
        use IEnumerator, allows for while loops and using WaitForSecondsRealtime() */
    private IEnumerator LoadAsync(int buildIndex) {
        /* If loading from menu */
        while (true) {
            /* Wait loadingTime seconds before loading scene */
            yield return new WaitForSecondsRealtime(loadingTime);

            AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex); //load scene asynchronously
            while (!operation.isDone)
            { //while loading not finished
                yield return null;
            }

            break;
        }

        loadingScreen.SetActive(false); //turn off load screen
    }

    /* Event called when new scene loaded */
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) { 
        ending = false;

        /* Play music if not already playing */
        if (music != null && !music.isPlaying) {
            music.Play(); 
        }

        Time.timeScale = 1;

        /* If new scene is not start menu */
        if (scene.buildIndex == MENU_SCENE) {
            /* Get start screen reference */
            startScreen = StartMenuManager.instance.startScreen;
        } else {
            // Get player reference
            player = PlayerFold.instance.transform.root.gameObject;

            if (player != null) {
                //get scripts for referencing
                playerSpawn = player.GetComponent<PlayerSpawn>();
            }

            cBrain = Camera.main.GetComponent<CinemachineBrain>(); //find camera brain for referencing
        }

        /* Set canvas camera to main camera */
        if (Camera.main != null && canvas != null) {
            canvas.worldCamera = Camera.main;
        }
    }


    /* Trigger fade */
    public void Fade() {
        canvas.transform.Find("Fade").gameObject
            .GetComponent<Animator>()
            .SetBool("fading", true);
    }

}