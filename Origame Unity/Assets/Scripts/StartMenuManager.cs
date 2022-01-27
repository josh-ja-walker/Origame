using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;

    void Start()
    {
        continueButton.SetActive(PlayerPrefs.HasKey("SavedPosX"));
    }

    public void ResetSavedPos()
    {
        PlayerPrefs.DeleteKey("SavedPosX");
        PlayerPrefs.DeleteKey("SavedPosY");
    }
}
