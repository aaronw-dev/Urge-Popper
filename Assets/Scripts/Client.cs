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
    [SerializeField]
    private string IDUrl = "https://big-balls-leaderboard.aw-dev.repl.co/getid";
    [ReadOnly]
    public string id = "READ FROM MEMORY";
    [ReadOnly]
    public string username = "READ FROM MEMORY";
    [ReadOnly]
    public string league = "READ FROM MEMORY";
    [ReadOnly]
    public string countryCode = "READ FROM MEMORY";
    [ReadOnly]
    public bool isNewUser = true;
    public GameObject m_NameField;
    public string FakeID;
    [ReadOnly]
    public Sprite flag;
    void Start()
    {
        SetID();
    }
    void Awake()
    {
        ActiveClient = this;
        DontDestroyOnLoad(gameObject);
    }
    void SetID()
    {
        isNewUser = !PlayerPrefs.HasKey("_id");
        if (PlayerPrefs.HasKey("_id"))
        {
#if UNITY_EDITOR
            id = FakeID == "" ? PlayerPrefs.GetString("_id") : FakeID;
#else
            id = PlayerPrefs.GetString("_id") ;
#endif
            StartCoroutine(fetchInformation());
            if (m_NameField)
                m_NameField.SetActive(false);
        }
        else
        {
            if (m_NameField)
                m_NameField.SetActive(true);
            StartCoroutine(getNewID());
        }
        StartCoroutine(fetchIPInformation());
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
                var userInformation = JSON.Parse(userJSON);
                countryCode = userInformation["countryCode"];

                var sprite = Resources.Load<Sprite>("Flags/" + countryCode.ToLower());
                flag = sprite;
            }
        }
    }
    public IEnumerator getNewID()
    {
        string returnedID = "";
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
                returnedID = request.downloadHandler.text;
            }
        }
        id = returnedID;
    }
    public IEnumerator fetchInformation()
    {
        if (!isNewUser)
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
                    countryCode = userInformation["country"];

                    var sprite = Resources.Load<Sprite>("Flags/" + countryCode.ToLower());
                    flag = sprite;
                    PlayerPrefs.SetString("_name", username);
                    PlayerPrefs.Save();
                    if (username == "" || league == "")
                    {
                        if (m_NameField)
                            m_NameField.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else
                    {
                        if (m_NameField)
                            m_NameField.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

}
