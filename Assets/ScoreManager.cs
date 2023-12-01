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
            if(scoreRoutine != null)
                StopCoroutine(scoreRoutine);
            scoreRoutine = StartCoroutine(ScoreCounter());
        }
    }
    void Awake()
    {
        Instance = this;
        StartCoroutine(GetIdentification());
    }
    IEnumerator  ScoreCounter()
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
      private static readonly string[] prefixes = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa",
                                                 "Lambda", "Mu", "Nu", "Xi", "Omicron", "Pi", "Rho", "Sigma", "Tau", "Upsilon",
                                                 "Phi", "Chi", "Psi", "Omega", "Star", "Nova", "Cosmic", "Quantum", "Galactic", "Solar",
                                                 "Lunar", "Nebula", "Aurora", "Supernova", "Infinity", "Zero", "Velocity", "Vortex", "Chrono", 
                                                 "Electro", "Hyper", "Sonic", "Cyber", "Mega", "Ultra", "Epic", "Master", "Super", "Ultimate","Nut",
                                                 "Cum", "Cumarde" };

    private static readonly string[] suffixes = { "Prime", "Max", "Neo", "Tech", "Bot", "X", "Pro", "Genius", "Legend", "Titan","Hunter",
                                                 "Master", "Wizard", "Champion", "King", "Queen", "Lord", "Queen", "Guru", "Legend", "Oracle",
                                                 "Sorcerer", "Warrior", "Pioneer", "Explorer", "Voyager", "Pilot", "Navigator", "Adventurer", "Seeker",
                                                 "Journeyer", "Conqueror", "Victor", "Triumph", "Era", "Epoch", "Dynasty", "Age", "Chronicle", "Empire",
                                                 "Alliance", "Union", "Syndicate", "Federation", "Unity", "Harmony", "Symphony", "Pinnacle", "Apex", "Zenith" };
    IEnumerator GameEndRoutine()
    {
        if(PlayerPrefs.GetString("_name","") == "")
        {
         PlayerPrefs.SetString("_name", prefixes[Random.Range(0, prefixes.Length)] + " " + suffixes[Random.Range(0, suffixes.Length)]);
         PlayerPrefs.Save();
        }
        string username =  PlayerPrefs.GetString("_name", "");
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
        LeaderboardManager.Instance.GetView();
    }
}
