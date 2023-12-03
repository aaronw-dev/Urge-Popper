using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TMP_Text scoreText;
    private int score = 0;
    string apiUrl = "https://big-balls-leaderboard.aw-dev.repl.co/uploadscore";
    string IDUrl = "https://big-balls-leaderboard.aw-dev.repl.co/getid";
    public string apiid = "";
    int scoreCounter = 0;
    Coroutine scoreRoutine;
    public int PublicScore
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            PlayerPrefs.SetInt("score", score);
            if (scoreRoutine != null)
                StopCoroutine(scoreRoutine);
            scoreRoutine = StartCoroutine(ScoreCounter());
        }
    }
    void Awake()
    {
        Instance = this;
        StartCoroutine(GetIdentification());
    }
    IEnumerator ScoreCounter()
    {
        while (scoreCounter < score)
        {
            scoreCounter += 25;
            scoreText.text = scoreCounter.ToString();
            scoreText.transform.DOPunchScale(Vector3.one * 0.1f, 0.025f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.025f);
        }
        scoreRoutine = null;
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
                request.SetRequestHeader("Access-Control-Allow-Origin", "*");
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
        string username = PlayerPrefs.GetString("_name", "");
        string id = apiid;
        string json = "{\"username\":\"" + username + "\", \"score\":" + score + ", \"id\":\"" + id + "\"}";
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
        LeaderboardManager.Instance.GetView();
    }
}
