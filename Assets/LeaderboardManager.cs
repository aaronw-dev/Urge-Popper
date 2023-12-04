using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
[System.Serializable]
public class PlayerData
{
    public string playerId;
    public int highScore;
    public int recentScore;
    public string username;
}

public class LeaderboardManager : MonoBehaviour
{
    public List<PlayerData> playerDataList = new List<PlayerData>();
    public List<PlayerData> leagueplayerDataList = new List<PlayerData>();
    string apiUrl = "https://big-balls-leaderboard.aw-dev.repl.co/leaderboard/";
    public Transform Container;
    public Transform LeagueContainer;
    public GameObject PlayerScorePrefab;
    public static LeaderboardManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    bool check = false;
    public void GetView()
    {
        if (check)
            return;

        check = true;
        StartCoroutine(FetchLeaderboard());
        StartCoroutine(FetchLeagueLeaderboard());
    }
    IEnumerator FetchLeaderboard()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + Client.ActiveClient.league))
        {
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string jsonString = request.downloadHandler.text;

                // Deserialize the JSON into a Dictionary<string, PlayerData>
                var playerDataDict = JSON.Parse(jsonString);
                // Extract values into the list, including the ID
                foreach (var kvp in playerDataDict)
                {
                    PlayerData playerData = new PlayerData();
                    playerData.highScore = kvp.Value["highScore"];
                    playerData.recentScore = kvp.Value["recentScore"];
                    playerData.username = kvp.Value["username"];
                    playerData.playerId = kvp.Key; // Assign the ID
                    playerDataList.Add(playerData);
                }
                playerDataList.Sort((a, b) => b.highScore.CompareTo(a.highScore));
                for (int i = 0; i < playerDataList.Count; i++)
                {
                    GameObject playerScoreObj = Instantiate(PlayerScorePrefab, Container);
                    playerScoreObj.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = (i + 1).ToString();
                    playerScoreObj.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = playerDataList[i].username;
                    playerScoreObj.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = playerDataList[i].highScore.ToString();
                    if (i <= 2)
                    {
                        playerScoreObj.transform.GetChild(3).GetChild(i).gameObject.SetActive(true);
                    }
                    if (playerDataList[i].playerId == Client.ActiveClient.id)
                    {
                        playerScoreObj.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
                    }
                }
            }
        }
    }

    IEnumerator FetchLeagueLeaderboard()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + Client.ActiveClient.league))
        {
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string jsonString = request.downloadHandler.text;

                // Deserialize the JSON into a Dictionary<string, PlayerData>
                var playerDataDict = JSON.Parse(jsonString);
                // Extract values into the list, including the ID
                foreach (var kvp in playerDataDict)
                {
                    PlayerData playerData = new PlayerData();
                    playerData.highScore = kvp.Value["highScore"];
                    playerData.recentScore = kvp.Value["recentScore"];
                    playerData.username = kvp.Value["username"];
                    playerData.playerId = kvp.Key; // Assign the ID
                    leagueplayerDataList.Add(playerData);
                }
                playerDataList.Sort((a, b) => b.highScore.CompareTo(a.highScore));
                for (int i = 0; i < playerDataList.Count; i++)
                {
                    GameObject playerScoreObj = Instantiate(PlayerScorePrefab, LeagueContainer);
                    playerScoreObj.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = (i + 1).ToString();
                    playerScoreObj.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = playerDataList[i].username;
                    playerScoreObj.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = playerDataList[i].highScore.ToString();
                    if (i <= 2)
                    {
                        playerScoreObj.transform.GetChild(3).GetChild(i).gameObject.SetActive(true);
                    }
                    if (playerDataList[i].playerId == Client.ActiveClient.id)
                    {
                        playerScoreObj.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
                    }
                }
            }
        }
    }
}

