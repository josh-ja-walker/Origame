using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Rendering;

public class Settings : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private AudioMixer audioMixer;

    //ui objects
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    [SerializeField] private float defaultVolume = 0.8f; //default volume when not set

    [Header("Fullscreen")]
    [SerializeField] private Toggle fullscreenToggle; //toggle for fullscreen

    [Header("Quality")]
    [SerializeField] private TMP_Dropdown qualityDropdown; //ui object for quality
    [SerializeField] private int defaultQuality = 3; //default quality index (very high)


    private void Start() {
        if (PlayerPrefs.HasKey("MasterVol")) //if master volume has been set before
        {
            //load up saved volume
            masterSlider.value = PlayerPrefs.GetFloat("MasterVol"); 
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVol"));
        }
        else //volume not been set before
        {
            masterSlider.value = defaultVolume; //set volume to default value
        }

        if (PlayerPrefs.HasKey("MusicVol"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVol"));
        }
        else
        {
            musicSlider.value = defaultVolume;
        }

        if (PlayerPrefs.HasKey("SFXVol"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVol"));
        }
        else
        {
            sfxSlider.value = defaultVolume;
        }

        if (PlayerPrefs.HasKey("QualityLevel")) //if quality been set before
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel"); //load up quality
        }
        else //quality has not been set before
        {
            qualityDropdown.value = defaultQuality; //set quality to defauolt
        }

        if (PlayerPrefs.HasKey("Fullscreen")) //fullscreen has been set before
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1; //set to saved value
        }
        else
        {
            fullscreenToggle.isOn = true; //set to default value
        }
    }


    #region Volume
    public void SetMasterVolume(float sliderValue) //called when value changed
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20); //set master channel value to slider value converted to logarithm
        PlayerPrefs.SetFloat("MasterVol", sliderValue); //save slider value
    }
    
    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVol", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }
    #endregion

    public void SetQualityLevel(int qualityInt) //called when quality changed
    {
        QualitySettings.SetQualityLevel(qualityInt); //set quality settings
        PlayerPrefs.SetInt("QualityLevel", qualityInt); //save quality 
    }

    public void SetFullscreen(bool isFullscreen) //called when fullscreen toggle changed
    {
        Screen.fullScreen = isFullscreen; //set fullscreen value
     
        //set fullscreen mode to fullscreen if true, or windowed if false
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed; 
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0); //save fullscreen value
    }

}

