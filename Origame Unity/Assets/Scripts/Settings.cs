using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private float defaultVolume = 0.8f;

    [SerializeField] private TMP_Dropdown qualityDropdown;
    private int defaultQuality = 4;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("MasterVol") || PlayerPrefs.HasKey("MusicVol") || PlayerPrefs.HasKey("SFXVol"))
        {
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVol"));
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVol"));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVol"));
        }
        else
        {
            SetMasterVolume(defaultVolume);
            masterSlider.value = defaultVolume;
        
            SetMusicVolume(defaultVolume);
            musicSlider.value = defaultVolume;
        
            SetSFXVolume(defaultVolume);
            sfxSlider.value = defaultVolume;
        }

        if (PlayerPrefs.HasKey("QualityLevel"))
        {
            SetQualityLevel(PlayerPrefs.GetInt("QualityLevel"));
        }
        else
        {
            SetQualityLevel(defaultQuality);
            qualityDropdown.value = defaultQuality;
        }
    }

    #region Volume
    public void SetMasterVolume(float sliderValue)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVol", sliderValue);
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

    public void SetQualityLevel(int qualityInt)
    {
        QualitySettings.SetQualityLevel(qualityInt);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}

