using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public static StartMenuManager instance;    

    [SerializeField] private Button continueButton;
    public GameObject startScreen;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        //continue button is interactable if player has a saved position
        continueButton.interactable = PlayerPrefs.HasKey("SavedPosX");
    }

    /* Load first level of game */
    public void StartGame() {
        GameManager.GM.LoadLevel(GameManager.LEVEL_SCENE);
    }

    /* Restart game by deleting saved progress */
    public void RestartGame()
    {
        GameManager.GM.ResetSavedPos();
        StartGame();
    }

    /* Quit game, called by button */
    public void Quit() {
        Application.Quit();
    }

    public void OpenOptions() {
        GameManager.GM.OptionsScreen.SetActive(true);
    }

}