using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Global;

    [ReadOnly]
    [Range(0, 1)]
    public float masterVolume;
    [ReadOnly]
    [Range(0, 1)]
    public float musicVolume;
    [ReadOnly]
    [Range(0, 1)]
    public float SFXVolume;
    void Awake()
    {
        Global = this;
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
