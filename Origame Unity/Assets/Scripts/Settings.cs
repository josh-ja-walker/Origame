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
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private float defaultVolume = 0.8f;

    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private int defaultQuality = 4;

    [SerializeField] private Toggle fullscreenToggle;

    private void Start()
    {
        //    if (PlayerPrefs.HasKey("MasterVol") || PlayerPrefs.HasKey("MusicVol") || PlayerPrefs.HasKey("SFXVol")
        //        || PlayerPrefs.HasKey("QualityLevel") || PlayerPrefs.HasKey("Fullscreen"))
        //    {
        //        if (PlayerPrefs.HasKey("MasterVol"))
        //        {
        //            float masterValue = PlayerPrefs.GetFloat("MasterVol");

        //            SetMasterVolume(masterValue);
        //            masterSlider.value = masterValue;
        //        }

        //        if (PlayerPrefs.HasKey("MusicVol"))
        //        {
        //            float musicValue = PlayerPrefs.GetFloat("MusicVol");

        //            SetMusicVolume(musicValue);
        //            musicSlider.value = musicValue;
        //        }

        //        if (PlayerPrefs.HasKey("SFXVol"))
        //        {
        //            float sfxValue = PlayerPrefs.GetFloat("SFXVol");

        //            SetSFXVolume(sfxValue);
        //            sfxSlider.value = sfxValue;
        //        }

        //        if (PlayerPrefs.HasKey("QualityLevel"))
        //        {
        //            int value = PlayerPrefs.GetInt("QualityLevel");

        //            SetQualityLevel(value);
        //            qualityDropdown.value = value;
        //        }

        //        if (PlayerPrefs.HasKey("Fullscreen"))
        //        {
        //            bool value = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen"));

        //            SetFullscreen(value);
        //            fullscreenToggle.isOn = (value);
        //        }
        //    }
        //    else
        //    {
        //        SetMasterVolume(defaultVolume);
        //        masterSlider.value = defaultVolume;

        //        SetMusicVolume(defaultVolume);
        //        musicSlider.value = defaultVolume;

        //        SetSFXVolume(defaultVolume);
        //        sfxSlider.value = defaultVolume;

        //        SetQualityLevel(defaultQuality);
        //        qualityDropdown.value = defaultQuality;

        //        SetFullscreen(true);
        //        fullscreenToggle.isOn = true;
        //    }


        if (PlayerPrefs.HasKey("MasterVol"))
        {
            masterSlider.value = PlayerPrefs.GetFloat("MasterVol");
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVol"));
        }
        else
        {
            masterSlider.value = defaultVolume;
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

        if (PlayerPrefs.HasKey("QualityLevel"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel");
        }
        else
        {
            qualityDropdown.value = defaultQuality;
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1 ? true : false;
        }
        else
        {
            fullscreenToggle.isOn = true;
        }
    }

    #region Volume
    public void SetMasterVolume(float sliderValue)
    {
        Debug.Log("Set Master");
        audioMixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVol", sliderValue);
    }
    
    public void SetMusicVolume(float sliderValue)
    {
        Debug.Log("Set Music");

        audioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVol", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        Debug.Log("Set SFX");

        audioMixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }
    #endregion

    public void SetQualityLevel(int qualityInt)
    {
        QualitySettings.SetQualityLevel(qualityInt);
        PlayerPrefs.SetInt("QualityLevel", qualityInt);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
}

