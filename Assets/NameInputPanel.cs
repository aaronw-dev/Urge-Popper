using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

public class NameInputPanel : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void Submit()
    {
        string randomPrefix = NamePrefixes.prefixes[Random.Range(0, NamePrefixes.prefixes.Length)];
        string randomSuffix = NamePrefixes.suffixes[Random.Range(0, NamePrefixes.suffixes.Length)];
        string name = nameInput.text.ToTitleCase();
        string combinedName = randomPrefix + " " + name;
        Debug.Log(combinedName);
        PlayerPrefs.SetString("_name", combinedName);
        PlayerPrefs.Save();
        this.gameObject.SetActive(false);
    }
}
