using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TMP_Text scoreText;
    private int score = 0;
    string apiUrl = "https://big-balls-leaderboard.aw-dev.repl.co/uploadscore";
    string IDUrl = "https://big-balls-leaderboard.aw-dev.repl.co/getid";
    public string apiid = "";
    public int PublicScore
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
    }
    void Awake()
    {
        Instance = this;
        StartCoroutine(GetIdentification());
    }

    public void GameEnd()
    {
        StartCoroutine(GameEndRoutine());
    }

    IEnumerator GetIdentification()
    {
        if (!PlayerPrefs.HasKey("_id"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(IDUrl))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + request.error);
                }
                else
                {
                    apiid = request.downloadHandler.text;
                    PlayerPrefs.SetString("_id", apiid);
                }
            }
        }
        else
        {
            apiid = PlayerPrefs.GetString("_id");
        }
    }
    IEnumerator GameEndRoutine()
    {
        string username = "aw the dev";
        string id = apiid;
        string json = "{\"username\":\"" + username + "\", \"score\":" + score + ", \"id\":\"" + id + "\"}";
        Debug.Log(json);
        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, json.ToString()))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Request successful!");
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
