using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
public class NameInputPanel : MonoBehaviour
{
    public TMP_InputField nameInput;
    string IDUrl = "https://big-balls-leaderboard.aw-dev.repl.co/getid";
    string apiUrl = "https://big-balls-leaderboard.aw-dev.repl.co/uploadscore";
    GameObject container;
    void Start()
    {
        container = transform.GetChild(0).gameObject;
        Client.ActiveClient.fetchInformation();
    }
    public void Submit()
    {
        string randomPrefix = NamePrefixes.prefixes[Random.Range(0, NamePrefixes.prefixes.Length)];
        string randomSuffix = NamePrefixes.suffixes[Random.Range(0, NamePrefixes.suffixes.Length)];
        string name = nameInput.text.ToTitleCase();
        string combinedName = randomPrefix + " " + name;
        PlayerPrefs.SetString("_name", combinedName);
        PlayerPrefs.Save();
        Client.ActiveClient.username = combinedName;
        StartCoroutine(CreateUserProfile());
    }
    [Button]
    public void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    IEnumerator CreateUserProfile()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(IDUrl))
        {
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Client.ActiveClient.id = request.downloadHandler.text;
                PlayerPrefs.SetString("_id", Client.ActiveClient.id);
            }
        }
        string username = Client.ActiveClient.username;
        string id = Client.ActiveClient.id;
        string league = "nutter";
        string json = "{\"username\":\"" + username + "\", \"league\":\"" + league + "\", \"score\":" + 0 + ", \"id\":\"" + id + "\"}";
        Debug.Log(json);
        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, json.ToString()))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
        }
        container.SetActive(false);
    }
}
