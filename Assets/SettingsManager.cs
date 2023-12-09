using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public void OnMasterVolumeChanged(float volume)
    {
        VolumeManager.Global.masterVolume = volume;
    }
    public void OnMusicVolumeChanged(float volume)
    {
        VolumeManager.Global.musicVolume = volume;
    }
    public void OnSFXVolumeChanged(float volume)
    {
        VolumeManager.Global.SFXVolume = volume;
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
}
