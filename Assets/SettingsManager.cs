using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public TMP_Text masterVolumeText;
    public Slider masterVolumeSlider;
    public TMP_Text musicVolumeText;
    public Slider musicVolumeSlider;
    public TMP_Text SFXVolumeText;
    public Slider SFXVolumeSlider;
    [ReadOnly]
    public bool isMenuOpen;
    private void Start()
    {
        masterVolumeSlider.value = VolumeManager.Global.masterVolume = PlayerPrefs.GetFloat("_mastervolume");
        musicVolumeSlider.value = VolumeManager.Global.musicVolume = PlayerPrefs.GetFloat("_musicvolume");
        SFXVolumeSlider.value = VolumeManager.Global.SFXVolume = PlayerPrefs.GetFloat("_sfxvolume");
    }

    public void OnMasterVolumeChanged(float volume)
    {
        masterVolumeText.text = (Math.Round(volume, 2) * 100).ToString() + "%";
        VolumeManager.Global.masterVolume = volume;
        PlayerPrefs.SetFloat("_mastervolume", volume);
        PlayerPrefs.Save();
    }
    public void OnMusicVolumeChanged(float volume)
    {
        musicVolumeText.text = volume.ToString();
        VolumeManager.Global.musicVolume = volume;
        PlayerPrefs.SetFloat("_musicvolume", volume);
        PlayerPrefs.Save();
    }
    public void OnSFXVolumeChanged(float volume)
    {
        SFXVolumeText.text = volume.ToString();
        VolumeManager.Global.SFXVolume = volume;
        PlayerPrefs.SetFloat("_sfxvolume", volume);
        PlayerPrefs.Save();
    }

    [Button]
    public void OpenMenu()
    {
        GetComponent<Animator>().Play("SettingsOpen");
    }
    [Button]
    public void CloseMenu()
    {
        GetComponent<Animator>().Play("SettingsClose");
    }
    [Button]
    public void ToggleMenu()
    {
        if (isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
        isMenuOpen = !isMenuOpen;
    }

}
