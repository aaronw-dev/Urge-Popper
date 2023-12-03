using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class RemoveNumbers : MonoBehaviour
{
    TMP_InputField inputField;
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    public void OnChangeText(string input)
    {
        inputField.text = Regex.Replace(input, @"[\d-]", string.Empty);
    }

}
