using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
public class Client : MonoBehaviour
{
    public static Client ActiveClient;
    [SerializeField]
    private string UserInformationUrl = "https://big-balls-leaderboard.aw-dev.repl.co/users/";
    [SerializeField]
    private string UserIPUrl = "https://big-balls-leaderboard.aw-dev.repl.co/findmyip";
    [ReadOnly]
    public string id = "READ FROM MEMORY";
    [ReadOnly]
    public string username = "READ FROM MEMORY";
    [ReadOnly]
    public string league = "READ FROM MEMORY";
    [ReadOnly]
    public string countryCode = "READ FROM MEMORY";
    public GameObject m_NameField;
    public string FakeID = "2j76QAJrqeriVYdQ7ZsD6N4IJZqDFS8ljYOWGAPtwTJTAvxhXO4HY0toL9kK23B3wJ9Cp";
    public Image flag;
    void Start()
    {
        if (m_NameField && PlayerPrefs.GetString("_name", "") == "")
            m_NameField.SetActive(true);

        SetID();
    }
    void Awake()
    {
        ActiveClient = this;
        DontDestroyOnLoad(gameObject);
    }
    void SetID()
    {
        if (PlayerPrefs.HasKey("_id"))
        {
#if UNITY_EDITOR
            id = FakeID == "" ? PlayerPrefs.GetString("_id") : FakeID;
#else
            id = PlayerPrefs.GetString("_id") ;
#endif
            StartCoroutine(fetchInformation());
            m_NameField.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            m_NameField.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public IEnumerator fetchIPInformation()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(UserIPUrl))
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
                Debug.Log(userJSON);
                var userInformation = JSON.Parse(userJSON);
                countryCode = userInformation["countryCode"];

                var sprite = Resources.Load<Sprite>("Flags/" + countryCode);
                flag.sprite = sprite;
                PlayerPrefs.SetString("_name", username);
                PlayerPrefs.Save();
            }
        }
    }
    public IEnumerator fetchInformation()
    {
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
                PlayerPrefs.Save();
                if (username == "" || league == "")
                {
                    m_NameField.transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    m_NameField.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        StartCoroutine(fetchIPInformation());
    }

}
