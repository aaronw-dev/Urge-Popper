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
            PlayerPrefs.Save();
            if (scoreRoutine != null)
                StopCoroutine(scoreRoutine);
            scoreRoutine = StartCoroutine(ScoreCounter());
        }
    }
    void Awake()
    {
        Instance = this;
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

    IEnumerator GameEndRoutine()
    {
        string username = Client.ActiveClient.username;
        string id = Client.ActiveClient.id;
        string league = Client.ActiveClient.league;
        string json = "{\"username\":\"" + username + "\", \"league\":\"" + league + "\", \"score\":" + score + ", \"id\":\"" + id + "\"}";
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
