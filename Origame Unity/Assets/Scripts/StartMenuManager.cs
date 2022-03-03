using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;

    void Start()
    {
        //set the continue alpha
        continueButton.GetComponent<Image>().color = new Color(continueButton.GetComponent<Image>().color.r,
            continueButton.GetComponent<Image>().color.g,
            continueButton.GetComponent<Image>().color.b, 
            PlayerPrefs.HasKey("SavedPosX") ? 1 : (50f / 256f)); //if has saved position, alpha is max; if not, set alpha to 50/256

        //continue button is interactable if player has a saved position
        continueButton.interactable = PlayerPrefs.HasKey("SavedPosX");
    }

    public void ResetSavedPos() //delete the saved position of the player
    {
        PlayerPrefs.DeleteKey("SavedPosX");
        PlayerPrefs.DeleteKey("SavedPosY");
    }

    public void LoadLevel() //called when start or continue pressed
    {
        GameManager.GM.LoadLevel(1);
    }

    public void OpenOptions()
    {
        GameManager.GM.OptionsScreen.SetActive(true); //when press options, turn on options screen
    }

    public void Quit() //quit game, called by button
    {
        Application.Quit();
    }
}