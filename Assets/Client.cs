using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
public class Client : MonoBehaviour
{
    public static Client ActiveClient;
    [SerializeField]
    private string IDUrl = "https://big-balls-leaderboard.aw-dev.repl.co/getid";
    [SerializeField]
    private string UserInformationUrl = "https://big-balls-leaderboard.aw-dev.repl.co/users/";
    [ReadOnly]
    public string id = "READ FROM MEMORY";
    [ReadOnly]
    public string username = "READ FROM MEMORY";
    [ReadOnly]
    public string league = "READ FROM MEMORY";
    void Start()
    {
        StartCoroutine(GetInformation());
    }
    void Awake()
    {
        ActiveClient = this;
        DontDestroyOnLoad(gameObject);
    }
    IEnumerator GetInformation()
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
                    id = request.downloadHandler.text;
                    PlayerPrefs.SetString("_id", id);
                }
            }
        }
        else
        {
            id = PlayerPrefs.GetString("_id");
        }
        using (UnityWebRequest request = UnityWebRequest.Get(UserInformationUrl + id))
        {
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string userJSON = request.downloadHandler.text;
                var userInformation = JSON.Parse(userJSON);
                league = userInformation["league"];
                username = userInformation["username"];
                PlayerPrefs.SetString("_name", username);
            }
        }
    }
}
